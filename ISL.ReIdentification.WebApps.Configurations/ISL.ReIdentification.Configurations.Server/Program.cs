// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Text.Json;
using ISL.Providers.Notifications.Abstractions;
using ISL.Providers.Notifications.GovukNotify.Models;
using ISL.Providers.Notifications.GovukNotify.Providers.Notifications;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Identifiers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.NECS;
using ISL.ReIdentification.Core.Brokers.Notifications;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Brokers.NECS;
using ISL.ReIdentification.Core.Models.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests;
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
            AddOrchestrationServices(builder.Services);
            AddCoordinationServices(builder.Services);

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
            builder.EnableLowerCamelCase();

            return builder.GetEdmModel();
        }

        private static void AddProviders(IServiceCollection services, IConfiguration configuration)
        {
            NotificationConfigurations notificationConfigurations = configuration
                .GetSection("notificationConfigurations")
                    .Get<NotificationConfigurations>();

            NotifyConfigurations notifyConfigurations = new NotifyConfigurations
            {
                ApiKey = notificationConfigurations.ApiKey
            };

            services.AddSingleton(notificationConfigurations);
            services.AddSingleton(notifyConfigurations);
            services.AddTransient<INotificationAbstractionProvider, NotificationAbstractionProvider>();
            services.AddTransient<INotificationProvider, GovukNotifyProvider>();
        }

        private static void AddBrokers(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();
            services.AddTransient<IIdentifierBroker, IdentifierBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
            services.AddTransient<IReIdentificationStorageBroker, ReIdentificationStorageBroker>();
            services.AddTransient<INotificationBroker, NotificationBroker>();

            NECSConfiguration necsConfigurations = configuration
                .GetSection("necsConfiguration")
                    .Get<NECSConfiguration>();

            NECSConfiguration necsConfiguration = new NECSConfiguration
            {
                ApiUrl = necsConfigurations.ApiUrl,
                ApiKey = necsConfigurations.ApiKey,
                ApiMaxBatchSize = necsConfigurations.ApiMaxBatchSize,
            };

            services.AddSingleton(necsConfigurations);
            services.AddSingleton(necsConfiguration);
            services.AddTransient<INECSBroker, NECSBroker>();
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
        }

        private static void AddProcessingServices(IServiceCollection services)
        { }

        private static void AddOrchestrationServices(IServiceCollection services)
        {
            services.AddTransient<IAccessOrchestrationService, AccessOrchestrationService>();
            services.AddTransient<IPersistanceOrchestrationService, PersistanceOrchestrationService>();
            services.AddTransient<IAccessOrchestrationService, AccessOrchestrationService>();
            services.AddTransient<IIdentificationOrchestrationService, IdentificationOrchestrationService>();
            services.AddTransient<ICsvIdentificationRequestService, CsvIdentificationRequestService>();
        }

        private static void AddCoordinationServices(IServiceCollection services)
        {
            services.AddTransient<IIdentificationCoordinationService, IdentificationCoordinationService>();
        }
    }
}
