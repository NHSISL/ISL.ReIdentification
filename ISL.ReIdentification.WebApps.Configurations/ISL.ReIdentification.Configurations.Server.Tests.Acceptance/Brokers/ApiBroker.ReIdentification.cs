// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.Accesses;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string reIdentificationRelativeUrl = "api/reidentification";

        public async ValueTask<AccessRequest> PostImpersonationContextRequestAsync(AccessRequest accessRequest) =>
            await this.apiFactoryClient.PostContentAsync($"{reIdentificationRelativeUrl}/impersonation", accessRequest);
    }
}
