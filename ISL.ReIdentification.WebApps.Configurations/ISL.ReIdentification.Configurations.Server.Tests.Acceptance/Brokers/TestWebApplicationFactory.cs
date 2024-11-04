// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using ISL.Providers.ReIdentification.Abstractions;
using ISL.Providers.ReIdentification.OfflineFileSources.Models;
using ISL.Providers.ReIdentification.OfflineFileSources.Providers.OfflineFileSources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Brokers
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile("appsettings.Development.json", optional: true)
                    .AddEnvironmentVariables();
            });

            builder.ConfigureServices((context, services) =>
            {
                OverrideSecurityForTesting(services);

                OverrideReIdentificationProviderForTesting(
                    services,
                    context.Configuration);
            });
        }

        private static void OverrideSecurityForTesting(IServiceCollection services)
        {
            // Remove existing authentication and authorization
            var authenticationDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(IAuthenticationSchemeProvider));

            if (authenticationDescriptor != null)
            {
                services.Remove(authenticationDescriptor);
            }

            // Override authentication and authorization
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestScheme";
                options.DefaultChallengeScheme = "TestScheme";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("TestPolicy", policy => policy.RequireAssertion(_ => true));
            });
        }

        private static void OverrideReIdentificationProviderForTesting(
            IServiceCollection services,
            IConfiguration configiration)
        {
            var reIdentificationDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(IReIdentificationProvider));

            if (reIdentificationDescriptor != null)
            {
                services.Remove(reIdentificationDescriptor);
            }

            OfflineSourceReIdentificationConfigurations offlineSourceReIdentificationConfigurations = configiration
                .GetSection("offlineSourceReIdentificationConfigurations")
                    .Get<OfflineSourceReIdentificationConfigurations>();

            services.AddSingleton(offlineSourceReIdentificationConfigurations);
            services.AddTransient<IReIdentificationProvider, OfflineFileSourceReIdentificationProvider>();
        }
    }
}
