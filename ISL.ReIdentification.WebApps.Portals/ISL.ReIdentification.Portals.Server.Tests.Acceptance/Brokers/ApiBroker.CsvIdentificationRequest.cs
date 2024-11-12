// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.CsvIdentificationRequests;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string csvIdentificationRequestRelativeUrl = "api/csvidentificationrequests";

        public async ValueTask<CsvIdentificationRequest> PostCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest) =>
            await this.apiFactoryClient.PostContentAsync(csvIdentificationRequestRelativeUrl, csvIdentificationRequest);

        public async ValueTask<List<CsvIdentificationRequest>> GetAllCsvIdentificationRequestsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<CsvIdentificationRequest>>(
                $"{csvIdentificationRequestRelativeUrl}/");

        public async ValueTask<List<CsvIdentificationRequest>> GetSpecificCsvIdentificationRequestByIdAsync(Guid lookupId) =>
            await this.apiFactoryClient.GetContentAsync<List<CsvIdentificationRequest>>(
                $"{csvIdentificationRequestRelativeUrl}?$filter=Id eq {lookupId}");

        public async ValueTask<CsvIdentificationRequest> GetCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId) =>
            await this.apiFactoryClient.GetContentAsync<CsvIdentificationRequest>(
                $"{csvIdentificationRequestRelativeUrl}/{csvIdentificationRequestId}");

        public async ValueTask<CsvIdentificationRequest> PutCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest) =>
            await this.apiFactoryClient.PutContentAsync(csvIdentificationRequestRelativeUrl, csvIdentificationRequest);

        public async ValueTask<CsvIdentificationRequest> DeleteCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId) =>
            await this.apiFactoryClient.DeleteContentAsync<CsvIdentificationRequest>(
                $"{csvIdentificationRequestRelativeUrl}/{csvIdentificationRequestId}");
    }
}
