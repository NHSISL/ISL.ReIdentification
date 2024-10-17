// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
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
            });
        }
    }
}
