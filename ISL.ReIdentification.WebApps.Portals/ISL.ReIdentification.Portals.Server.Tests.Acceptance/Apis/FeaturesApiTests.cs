// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class FeaturesApiTests
    {
        private readonly ApiBroker apiBroker;

        public FeaturesApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;
    }
}