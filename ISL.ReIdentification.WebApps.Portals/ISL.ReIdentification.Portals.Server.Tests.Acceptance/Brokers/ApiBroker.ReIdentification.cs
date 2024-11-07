// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.Accesses;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string reIdentificationRelativeUrl = "api/reidentification";

        public async ValueTask<AccessRequest> PostIdentificationRequestsAsync(AccessRequest request) =>
            throw new NotImplementedException();


        public async ValueTask<AccessRequest> PostImpersonationContextRequestAsync(AccessRequest accessRequest) =>
            await this.apiFactoryClient.PostContentAsync($"{reIdentificationRelativeUrl}/impersonation", accessRequest);
    }
}
