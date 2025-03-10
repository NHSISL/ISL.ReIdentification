// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using ISL.Providers.Notifications.Abstractions;
using ISL.Providers.Notifications.GovukNotify.Models;
using ISL.Providers.Notifications.GovukNotify.Providers.Notifications;
using ISL.Providers.ReIdentification.Abstractions;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS;
using ISL.Providers.ReIdentification.Necs.Providers.NecsReIdentifications;
using ISL.Providers.ReIdentification.OfflineFileSources.Models;
using ISL.Providers.ReIdentification.OfflineFileSources.Providers.OfflineFileSources;
using ISL.Providers.Storages.Abstractions;
using ISL.Providers.Storages.AzureBlobStorage.Models;
using ISL.Providers.Storages.AzureBlobStorage.Providers.AzureBlobStorage;
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
using ISL.ReIdentification.Core.Services.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Services.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using ISL.ReIdentification.Core.Services.Orchestrations.Persists;
using ISL.ReIdentification.Core.Services.Processings.UserAccesses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = new HostBuilder()
        .ConfigureAppConfiguration(config =>
        {
            var env = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT");
            config.SetBasePath(Directory.GetCurrentDirectory())

            .AddJsonFile(path: "appsettings.json")
            .AddJsonFile(
                path: $"appsettings.{env}.json",
                optional: true)
            .AddJsonFile(
                path: "appsettings.local.json",
                optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables();
        })
        .ConfigureServices((context, services) =>
        {
            var configuration = context.Configuration;

            services
                .AddLogging(setup =>
                {
                    setup.AddApplicationInsights();
                    setup.AddConsole();
                });

            CsvReIdentificationConfigurations csvReIdentificationConfigurations =
            configuration
                .GetSection("CsvReIdentificationConfigurations")
                    .Get<CsvReIdentificationConfigurations>();

            services.AddSingleton(csvReIdentificationConfigurations);

            AddProviders(services, configuration);
            AddBrokers(services, configuration);
            AddFoundationServices(services);
            AddProcessingServices(services);
            AddOrchestrationServices(services, configuration);
            AddCoordinationServices(services, configuration);
        })
        .UseDefaultServiceProvider(options => options.ValidateScopes = false)
        .ConfigureFunctionsWorkerDefaults()
        .Build();

        await host.RunAsync();
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

        ProjectStorageConfiguration projectStorageConfiguration = configuration
            .GetSection("ProjectStorageConfiguration")
                .Get<ProjectStorageConfiguration>();

        AzureBlobStoreConfigurations projectsBlobStoreConfigurations = new AzureBlobStoreConfigurations
        {
            ServiceUri = projectStorageConfiguration.ServiceUri,
            StorageAccountName = projectStorageConfiguration.StorageAccountName,
            StorageAccountAccessKey = projectStorageConfiguration.StorageAccountAccessKey
        };

        services.AddSingleton(projectsBlobStoreConfigurations);
        services.AddTransient<INotificationAbstractionProvider, NotificationAbstractionProvider>();
        services.AddTransient<IStorageAbstractionProvider, StorageAbstractionProvider>();
        services.AddTransient<INotificationProvider, GovukNotifyProvider>();
        services.AddTransient<IStorageProvider, AzureBlobStorageProvider>();

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
        var credential = new DefaultAzureCredential();
        var tokenRequestContext = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
        AccessToken accessToken = credential.GetTokenAsync(tokenRequestContext).Result;
        SecurityBroker securityBroker = new SecurityBroker(accessToken.Token);

        services.AddTransient<ISecurityBroker>(broker => securityBroker);
        services.AddTransient<ILoggingBroker, LoggingBroker>();
        services.AddTransient<IDateTimeBroker, DateTimeBroker>();
        services.AddTransient<IIdentifierBroker, IdentifierBroker>();
        services.AddTransient<ILoggingBroker, LoggingBroker>();
        services.AddTransient<ICsvHelperBroker, CsvHelperBroker>();
        services.AddTransient<IReIdentificationStorageBroker, ReIdentificationStorageBroker>();
        services.AddTransient<INotificationBroker, NotificationBroker>();
        services.AddTransient<IHashBroker, HashBroker>();
        services.AddTransient<IBlobStorageBroker, BlobStorageBroker>();
        services.AddSingleton<IReIdentificationBroker, ReIdentificationBroker>();
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
        services.AddTransient<IUserAgreementService, UserAgreementService>();
    }

    private static void AddProcessingServices(IServiceCollection services)
    {
        services.AddTransient<IUserAccessProcessingService, UserAccessProcessingService>();
    }

    private static void AddOrchestrationServices(IServiceCollection services, IConfiguration configuration)
    {
        CsvReIdentificationConfigurations csvReIdentificationConfigurations = configuration
            .GetSection("CsvReIdentificationConfigurations")
                .Get<CsvReIdentificationConfigurations>() ??
                    new CsvReIdentificationConfigurations();

        services.AddSingleton(csvReIdentificationConfigurations);
        services.AddTransient<IAccessOrchestrationService, AccessOrchestrationService>();
        services.AddTransient<IPersistanceOrchestrationService, PersistanceOrchestrationService>();
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