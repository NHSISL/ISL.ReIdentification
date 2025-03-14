// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Brokers;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.ReIdentification.Brokers;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis.CsvIdentificationRequests
{
    [Collection(nameof(ApiTestCollection))]
    public partial class CsvIdentificationRequestApiTests
    {
        private readonly ApiBroker apiBroker;

        public CsvIdentificationRequestApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;
    }
}
