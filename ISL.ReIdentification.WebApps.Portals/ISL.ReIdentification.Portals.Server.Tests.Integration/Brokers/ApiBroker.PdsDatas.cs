﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------



using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.PdsDatas;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string PdsDataRelativeUrl = "api/pdsData";

        public async ValueTask<PdsData> PostPdsDataAsync(
            PdsData pdsData) =>
                await this.apiFactoryClient.PostContentAsync(PdsDataRelativeUrl, pdsData);

        public async ValueTask<PdsData> DeletePdsDataByIdAsync(Guid pdsDataId) =>
            await this.apiFactoryClient.DeleteContentAsync<PdsData>(
                $"{PdsDataRelativeUrl}/{pdsDataId}");
    }
}
