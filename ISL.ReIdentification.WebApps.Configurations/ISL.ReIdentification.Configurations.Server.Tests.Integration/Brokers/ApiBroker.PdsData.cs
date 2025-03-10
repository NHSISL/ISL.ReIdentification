// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.PdsData;

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string pdsDataRelativeUrl = "api/pdsData";

        public async ValueTask<PdsData> PostPdsDataAsync(
            PdsData pdsData) =>
                await this.apiFactoryClient.PostContentAsync(pdsDataRelativeUrl, pdsData);

        public async ValueTask<List<PdsData>> GetAllPdsDataAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<PdsData>>($"{pdsDataRelativeUrl}/");

        public async ValueTask<List<PdsData>> GetSpecificPdsDataByIdAsync(Guid pdsDataId) =>
            await this.apiFactoryClient.GetContentAsync<List<PdsData>>(
                $"{pdsDataRelativeUrl}?$filter=Id eq {pdsDataId}");

        public async ValueTask<PdsData> GetPdsDataByIdAsync(Guid pdsDataId) =>
            await this.apiFactoryClient.GetContentAsync<PdsData>($"{pdsDataRelativeUrl}/{pdsDataId}");

        public async ValueTask<PdsData> PutPdsDataAsync(PdsData pdsData) =>
            await this.apiFactoryClient.PutContentAsync(pdsDataRelativeUrl, pdsData);

        public async ValueTask<PdsData> DeletePdsDataByIdAsync(Guid pdsDataId) =>
            await this.apiFactoryClient.DeleteContentAsync<PdsData>(
                $"{pdsDataRelativeUrl}/{pdsDataId}");
    }
}
