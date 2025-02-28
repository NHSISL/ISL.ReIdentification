﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration
{
    public class ReleaseCandidateTestCaseDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink diagnosticMessageSink;
        private readonly IConfiguration configuration;

        public ReleaseCandidateTestCaseDiscoverer(IMessageSink diagnosticMessageSink)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            configuration = configurationBuilder.Build();
        }

        public IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo factAttribute)
        {
            var isReleaseCandidate = configuration.GetValue<bool>("IS_RELEASE_CANDIDATE");

            if (isReleaseCandidate)
            {
                yield return new XunitTestCase(
                    diagnosticMessageSink,
                    defaultMethodDisplay: discoveryOptions.MethodDisplayOrDefault(),
                    defaultMethodDisplayOptions: discoveryOptions.MethodDisplayOptionsOrDefault(),
                    testMethod);
            }
        }
    }
}
