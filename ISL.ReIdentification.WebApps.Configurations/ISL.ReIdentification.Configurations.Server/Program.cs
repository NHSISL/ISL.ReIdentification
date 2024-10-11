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
using ISL.ReIdentification.Core.Brokers.Notifications;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Services.Foundations.Lookups;
using ISL.ReIdentification.Core.Services.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Services.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Services.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Accesses;
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
            var builder =
                WebApplication.CreateBuilder(args);

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
            AddBrokers(builder.Services);
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

            static IEdmModel GetEdmModel()
            {
                ODataConventionModelBuilder builder =
                   new ODataConventionModelBuilder();

                builder.EntitySet<Lookup>("Lookups");
                return builder.GetEdmModel();
            }


            var app = builder.Build();
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

            app.Run();
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

        private static void AddBrokers(IServiceCollection services)
        {
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();
            services.AddTransient<IIdentifierBroker, IdentifierBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
            services.AddTransient<IReIdentificationStorageBroker, ReIdentificationStorageBroker>();
            services.AddTransient<INotificationBroker, NotificationBroker>();
        }

        private static void AddFoundationServices(IServiceCollection services)
        {
            services.AddTransient<IAccessAuditService, AccessAuditService>();
            services.AddTransient<IImpersonationContextService, ImpersonationContextService>();
            services.AddTransient<ILookupService, LookupService>();
            services.AddTransient<IOdsDataService, OdsDataService>();
            services.AddTransient<IPdsDataService, PdsDataService>();
            services.AddTransient<IUserAccessService, UserAccessService>();
        }

        private static void AddProcessingServices(IServiceCollection services)
        { }

        private static void AddOrchestrationServices(IServiceCollection services)
        {
            services.AddTransient<IAccessOrchestrationService, AccessOrchestrationService>();
        }

        private static void AddCoordinationServices(IServiceCollection services)
        { }
    }
}
