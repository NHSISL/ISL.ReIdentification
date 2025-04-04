﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.OdsData;

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string odsDataRelativeUrl = "api/odsData";

        public async ValueTask<OdsData> PostOdsDataAsync(OdsData odsData) =>
            await this.apiFactoryClient.PostContentAsync(odsDataRelativeUrl, odsData);

        public async ValueTask<List<OdsData>> GetAllOdsDatasAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<OdsData>>($"{odsDataRelativeUrl}");

        public async ValueTask<List<OdsData>> GetSpecificOdsDataByIdAsync(Guid odsDataId) =>
            await this.apiFactoryClient.GetContentAsync<List<OdsData>>(
                $"{odsDataRelativeUrl}?$filter=Id eq {odsDataId}");

        public async ValueTask<OdsData> GetOdsDataByIdAsync(Guid odsDataId) =>
            await this.apiFactoryClient.GetContentAsync<OdsData>($"{odsDataRelativeUrl}/{odsDataId}");

        public async ValueTask<OdsData> PutOdsDataAsync(OdsData odsData) =>
            await this.apiFactoryClient.PutContentAsync(odsDataRelativeUrl, odsData);

        public async ValueTask<List<OdsData>> GetChildrenAsync(Guid odsDataId) =>
            await this.apiFactoryClient.GetContentAsync<List<OdsData>>($"{odsDataRelativeUrl}/GetChildren/{odsDataId}");

        public async ValueTask<List<OdsData>> GetDescendantsAsync(Guid odsDataId) =>
            await this.apiFactoryClient.GetContentAsync<List<OdsData>>($"{odsDataRelativeUrl}/GetDescendants/{odsDataId}");

        public async ValueTask<List<OdsData>> GetAncestorsAsync(Guid odsDataId) =>
            await this.apiFactoryClient.GetContentAsync<List<OdsData>>($"{odsDataRelativeUrl}/GetAncestors/{odsDataId}");

        public async ValueTask<OdsData> DeleteOdsDataByIdAsync(Guid odsDataId) =>
            await this.apiFactoryClient.DeleteContentAsync<OdsData>($"{odsDataRelativeUrl}/{odsDataId}");
    }
}
