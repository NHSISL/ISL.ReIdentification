// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.Lookup;

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string csvIdentificationRequestRelativeUrl = "api/csvidentificationrequests";

        public async ValueTask<List<CsvIdentificationRequest>> GetAllCsvIdentificationRequestsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<CsvIdentificationRequest>>(csvIdentificationRequestRelativeUrl);
    }
}