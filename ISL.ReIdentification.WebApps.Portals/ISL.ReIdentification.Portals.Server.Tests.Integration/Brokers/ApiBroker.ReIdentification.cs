// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------



using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.Accesses;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string reIdentificationRelativeUrl = "api/reidentification";

        public async ValueTask<AccessRequest> PostIdentificationRequestsAsync(AccessRequest accessRequest) =>
            await this.apiFactoryClient.PostContentAsync($"{reIdentificationRelativeUrl}", accessRequest);

        public async ValueTask<AccessRequest> PostCsvIdentificationRequestAsync(AccessRequest accessRequest) =>
            await this.apiFactoryClient.PostContentAsync($"{reIdentificationRelativeUrl}/submitcsv", accessRequest);
    }
}
