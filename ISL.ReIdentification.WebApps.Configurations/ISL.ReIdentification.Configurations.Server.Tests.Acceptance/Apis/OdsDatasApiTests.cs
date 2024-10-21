// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Brokers;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class OdsDatasApiTests
    {
        private readonly ApiBroker apiBroker;

        public OdsDatasApiTests(ApiBroker apiBroker)
        {
            this.apiBroker = apiBroker;
        }
    }
}
