// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.PdsDatas;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string PdsDataRelativeUrl = "api/pdsData";

        public async ValueTask<PdsData> PostPdsDataAsync(
            PdsData pdsData) =>
                await this.apiFactoryClient.PostContentAsync(PdsDataRelativeUrl, pdsData);

        public async ValueTask<List<PdsData>> GetAllPdsDataAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<PdsData>>($"{PdsDataRelativeUrl}/");

        public async ValueTask<PdsData> GetPdsDataByIdAsync(Guid pdsDataId) =>
            await this.apiFactoryClient.GetContentAsync<PdsData>($"{PdsDataRelativeUrl}/{pdsDataId}");

        public async ValueTask<PdsData> PutPdsDataAsync(PdsData pdsData) =>
            throw new NotImplementedException();

        public async ValueTask<PdsData> DeletePdsDataByIdAsync(Guid pdsDataId) =>
            await this.apiFactoryClient.DeleteContentAsync<PdsData>(
                $"{PdsDataRelativeUrl}/{pdsDataId}");
    }
}
