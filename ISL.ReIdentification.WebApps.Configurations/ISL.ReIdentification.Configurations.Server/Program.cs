// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Text.Json;
using ISL.Providers.Notifications.Abstractions;
using ISL.Providers.Notifications.GovukNotify.Models;
using ISL.Providers.Notifications.GovukNotify.Providers.Notifications;
using ISL.Providers.ReIdentification.Abstractions;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS;
using ISL.Providers.ReIdentification.Necs.Providers.NecsReIdentifications;
using ISL.Providers.ReIdentification.OfflineFileSources.Models;
using ISL.Providers.ReIdentification.OfflineFileSources.Providers.OfflineFileSources;
using ISL.ReIdentification.Core.Brokers.CsvHelpers;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Hashing;
using ISL.ReIdentification.Core.Brokers.Identifiers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Notifications;
using ISL.ReIdentification.Core.Brokers.ReIdentifications;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Brokers.Storages.Blob;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Services.Foundations.Documents;
using ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Services.Foundations.Lookups;
using ISL.ReIdentification.Core.Services.Foundations.Notifications;
using ISL.ReIdentification.Core.Services.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Services.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Services.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Services.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using ISL.ReIdentification.Core.Services.Orchestrations.Persists;
using ISL.ReIdentification.Core.Services.Processings.UserAccesses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace ISL.ReIdentification.Configurations.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder, builder.Configuration);
            var app = builder.Build();
            ConfigurePipeline(app);
            app.Run();
        }

        public static void ConfigureServices(WebApplicationBuilder builder, IConfiguration configuration)
        {
            // Load settings from launchSettings.json (for testing)
            var projectDir = Directory.GetCurrentDirectory();
            var launchSettingsPath = Path.Combine(projectDir, "Properties", "launchSettings.json");

            if (File.Exists(launchSettingsPath))
            {
                builder.Configuration.AddJsonFile(launchSettingsPath);
            }

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange:true)
                .AddEnvironmentVariables();

            // Add services to the container.
            var azureAdOptions = builder.Configuration.GetSection("AzureAd");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(azureAdOptions);

            builder.Services.AddAuthorization();
            builder.Services.AddDbContext<ReIdentificationStorageBroker>();
            builder.Services.AddHttpContextAccessor();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();
            AddProviders(builder.Services, builder.Configuration);
            AddBrokers(builder.Services, builder.Configuration);
            AddFoundationServices(builder.Services);
            AddProcessingServices(builder.Services);
            AddOrchestrationServices(builder.Services, builder.Configuration);
            AddCoordinationServices(builder.Services, builder.Configuration);

            // Register IConfiguration to be available for dependency injection
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            JsonNamingPolicy jsonNamingPolicy = JsonNamingPolicy.CamelCase;

            builder.Services.AddControllers()
               .AddOData(options =>
               {
                   options.AddRouteComponents("odata", GetEdmModel());
                   options.Select().Filter().Expand().OrderBy().Count().SetMaxTop(100);
               })
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.PropertyNamingPolicy = jsonNamingPolicy;
                   options.JsonSerializerOptions.DictionaryKeyPolicy = jsonNamingPolicy;
                   options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                   options.JsonSerializerOptions.WriteIndented = true;
               });
        }

        public static void ConfigurePipeline(WebApplication app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers().WithOpenApi();
            app.MapFallbackToFile("/index.html");
        }

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder =
               new ODataConventionModelBuilder();

            builder.EntitySet<Lookup>("Lookups");
            builder.EntitySet<UserAccess>("UserAccesses");
            builder.EntitySet<ImpersonationContext>("ImpersonationContexts");
            builder.EntitySet<CsvIdentificationRequest>("CsvIdentificationRequests");
            builder.EntitySet<OdsData>("OdsData");
            builder.EntitySet<PdsData>("PdsData");
            builder.EntitySet<AccessAudit>("AccessAudits");
            builder.EnableLowerCamelCase();

            return builder.GetEdmModel();
        }

        private static void AddProviders(IServiceCollection services, IConfiguration configuration)
        {
            NotificationConfigurations notificationConfigurations = configuration
                .GetSection("NotificationConfigurations")
                    .Get<NotificationConfigurations>();

            NotifyConfigurations notifyConfigurations = new NotifyConfigurations
            {
                ApiKey = notificationConfigurations.ApiKey
            };

            services.AddSingleton(notificationConfigurations);
            services.AddSingleton(notifyConfigurations);
            services.AddTransient<INotificationAbstractionProvider, NotificationAbstractionProvider>();
            services.AddTransient<INotificationProvider, GovukNotifyProvider>();

            bool reIdentificationProviderOfflineMode = configuration
                .GetSection("ReIdentificationProviderOfflineMode").Get<bool>();

            if (reIdentificationProviderOfflineMode == true)
            {
                OfflineSourceReIdentificationConfigurations offlineSourceReIdentificationConfigurations = configuration
                    .GetSection("OfflineSourceReIdentificationConfigurations")
                        .Get<OfflineSourceReIdentificationConfigurations>();

                services.AddSingleton(offlineSourceReIdentificationConfigurations);
                services.AddTransient<IReIdentificationProvider, OfflineFileSourceReIdentificationProvider>();
            }
            else
            {
                NecsReIdentificationConfigurations necsReIdentificationConfigurations = configuration
                    .GetSection("NecsReIdentificationConfigurations")
                        .Get<NecsReIdentificationConfigurations>();

                services.AddSingleton(necsReIdentificationConfigurations);
                services.AddTransient<IReIdentificationProvider, NecsReIdentificationProvider>();
            }

            services.AddTransient<IReIdentificationAbstractionProvider, ReIdentificationAbstractionProvider>();
        }

        private static void AddBrokers(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();
            services.AddTransient<IIdentifierBroker, IdentifierBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
            services.AddTransient<ICsvHelperBroker, CsvHelperBroker>();
            services.AddTransient<ISecurityBroker, SecurityBroker>();
            services.AddTransient<IReIdentificationStorageBroker, ReIdentificationStorageBroker>();
            services.AddTransient<INotificationBroker, NotificationBroker>();
            services.AddTransient<IHashBroker, HashBroker>();
            services.AddTransient<IBlobStorageBroker, BlobStorageBroker>();
            services.AddTransient<IReIdentificationBroker, ReIdentificationBroker>();
        }

        private static void AddFoundationServices(IServiceCollection services)
        {
            services.AddTransient<IAccessAuditService, AccessAuditService>();
            services.AddTransient<IImpersonationContextService, ImpersonationContextService>();
            services.AddTransient<ILookupService, LookupService>();
            services.AddTransient<IOdsDataService, OdsDataService>();
            services.AddTransient<IPdsDataService, PdsDataService>();
            services.AddTransient<IUserAccessService, UserAccessService>();
            services.AddTransient<IImpersonationContextService, ImpersonationContextService>();
            services.AddTransient<ICsvIdentificationRequestService, CsvIdentificationRequestService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IReIdentificationService, ReIdentificationService>();
            services.AddTransient<IDocumentService, DocumentService>();
        }

        private static void AddProcessingServices(IServiceCollection services)
        {
            services.AddTransient<IUserAccessProcessingService, UserAccessProcessingService>();
        }

        private static void AddOrchestrationServices(IServiceCollection services, IConfiguration configuration)
        {
            CsvReIdentificationConfigurations csvReIdentificationConfigurations = configuration
                .GetSection("csvReIdentificationConfigurations")
                    .Get<CsvReIdentificationConfigurations>() ??
                        new CsvReIdentificationConfigurations();

            services.AddSingleton(csvReIdentificationConfigurations);
            services.AddTransient<IAccessOrchestrationService, AccessOrchestrationService>();
            services.AddTransient<IPersistanceOrchestrationService, PersistanceOrchestrationService>();
            services.AddTransient<IAccessOrchestrationService, AccessOrchestrationService>();
            services.AddTransient<IIdentificationOrchestrationService, IdentificationOrchestrationService>();
            services.AddTransient<ICsvIdentificationRequestService, CsvIdentificationRequestService>();
        }

        private static void AddCoordinationServices(IServiceCollection services, IConfiguration configuration)
        {
            ProjectStorageConfiguration projectStorageConfiguration = configuration
                .GetSection("ProjectStorageConfiguration")
                    .Get<ProjectStorageConfiguration>();

            services.AddSingleton(projectStorageConfiguration);

            services.AddTransient<IIdentificationCoordinationService, IdentificationCoordinationService>();
        }
    }
}
