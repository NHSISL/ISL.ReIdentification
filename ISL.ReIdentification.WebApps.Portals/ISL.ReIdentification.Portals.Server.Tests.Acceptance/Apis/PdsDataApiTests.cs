// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PdsDataApiTests
    {
        private readonly ApiBroker apiBroker;

        public PdsDataApiTests(ApiBroker apiBroker)
        {
            this.apiBroker = apiBroker;
        }
    }
}
