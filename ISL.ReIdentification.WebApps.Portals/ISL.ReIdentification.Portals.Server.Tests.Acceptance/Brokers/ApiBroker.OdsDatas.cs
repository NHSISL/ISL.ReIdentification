// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.OdsDatas;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string odsDataRelativeUrl = "api/odsData";

        public async ValueTask<OdsData> PostOdsDataAsync(OdsData odsData) =>
            await this.apiFactoryClient.PostContentAsync(odsDataRelativeUrl, odsData);

        public async ValueTask<List<OdsData>> GetAllOdsDatasAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<OdsData>>($"{odsDataRelativeUrl}");

        public async ValueTask<OdsData> GetOdsDataByIdAsync(Guid odsDataId) =>
            await this.apiFactoryClient.GetContentAsync<OdsData>($"{odsDataRelativeUrl}/{odsDataId}");

        public async ValueTask<OdsData> DeleteOdsDataByIdAsync(Guid odsDataId) =>
            await this.apiFactoryClient.DeleteContentAsync<OdsData>($"{odsDataRelativeUrl}/{odsDataId}");
    }
}
