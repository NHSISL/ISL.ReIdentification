// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Brokers;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.ReIdentification.Brokers;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class FrontendConfigurationsApiTests
    {
        private readonly ApiBroker apiBroker;

        public FrontendConfigurationsApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;
    }
}
