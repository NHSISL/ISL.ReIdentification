// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using ISL.Providers.Notifications.Abstractions;
using ISL.Providers.Notifications.GovukNotify.Models;
using ISL.Providers.Notifications.GovukNotify.Providers.Notifications;
using ISL.Providers.ReIdentification.Abstractions;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.Notifications;
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
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Services.Foundations.Notifications;
using ISL.ReIdentification.Core.Services.Orchestrations.Persists;
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
                .GetSection("csvReIdentificationConfigurations")
                    .Get<CsvReIdentificationConfigurations>();

            services.AddSingleton(csvReIdentificationConfigurations);

            AddProviders(services, configuration);
            AddBrokers(services);
            AddServices(services);
            AddServices(services);
            AddProcessings(services);
            AddOrchestrations(services);
            AddCoordinations(services);
        })
        .UseDefaultServiceProvider(options => options.ValidateScopes = false)
        .ConfigureFunctionsWorkerDefaults()
        .Build();

        await host.RunAsync();
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

        ProjectStorageConfiguration projectStorageConfiguration = configuration
            .GetSection("projectStorageConfiguration")
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
            .GetSection("reIdentificationProviderOfflineMode").Get<bool>();

        if (reIdentificationProviderOfflineMode == true)
        {
            OfflineSourceReIdentificationConfigurations offlineSourceReIdentificationConfigurations = configuration
                .GetSection("offlineSourceReIdentificationConfigurations")
                    .Get<OfflineSourceReIdentificationConfigurations>();

            services.AddSingleton(offlineSourceReIdentificationConfigurations);
            services.AddTransient<IReIdentificationProvider, OfflineFileSourceReIdentificationProvider>();
        }
        else
        {
            NecsReIdentificationConfigurations necsReIdentificationConfigurations = configuration
                .GetSection("necsReIdentificationConfigurations")
                    .Get<NecsReIdentificationConfigurations>();

            services.AddSingleton(necsReIdentificationConfigurations);
            services.AddTransient<IReIdentificationProvider, NecsReIdentificationProvider>();
        }

        services.AddTransient<IReIdentificationAbstractionProvider, ReIdentificationAbstractionProvider>();
    }

    private static void AddBrokers(IServiceCollection services)
    {
        services.AddTransient<ILoggingBroker, LoggingBroker>();
        services.AddTransient<IDateTimeBroker, DateTimeBroker>();
        services.AddTransient<IIdentifierBroker, IdentifierBroker>();
        services.AddTransient<IReIdentificationStorageBroker, ReIdentificationStorageBroker>();
        services.AddTransient<IHashBroker, HashBroker>();
        services.AddTransient<INotificationBroker, INotificationBroker>();
        services.AddTransient<ICsvHelperBroker, CsvHelperBroker>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddTransient<IImpersonationContextService, ImpersonationContextService>();
        services.AddTransient<ICsvIdentificationRequestService, CsvIdentificationRequestService>();
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IAccessAuditService, AccessAuditService>();
    }

    private static void AddProcessings(IServiceCollection services)
    { }

    private static void AddOrchestrations(IServiceCollection services)
    {
        services.AddTransient<IPersistanceOrchestrationService, PersistanceOrchestrationService>();
    }

    private static void AddCoordinations(IServiceCollection services)
    {
        services.AddTransient<IIdentificationCoordinationService, IdentificationCoordinationService>();
    }
}