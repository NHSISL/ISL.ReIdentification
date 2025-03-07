// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.AccessAudit;

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string featuresRelativeUrl = "api/Features";

        public async ValueTask<string[]> GetFeaturesAsync() =>
            await this.apiFactoryClient.GetContentAsync<string[]>(featuresRelativeUrl);
    }
}