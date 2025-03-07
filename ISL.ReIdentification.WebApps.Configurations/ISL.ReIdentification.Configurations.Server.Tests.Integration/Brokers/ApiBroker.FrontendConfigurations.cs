// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string frontendConfigurationsRelativeUrl = "api/frontendConfigurations";

        public async ValueTask<Dictionary<string, string>> GetFrontendConfigurationsAsync() =>
            await this.apiFactoryClient.GetContentAsync<Dictionary<string, string>>(frontendConfigurationsRelativeUrl);
    }
}