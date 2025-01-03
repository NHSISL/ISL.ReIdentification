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

        public async ValueTask<AccessRequest> PostIdentificationRequestsAsync(AccessRequest accessRequest) =>
            await this.apiFactoryClient.PostContentAsync($"{reIdentificationRelativeUrl}", accessRequest);

        public async ValueTask<AccessRequest> PostImpersonationContextRequestAsync(AccessRequest accessRequest) =>
            await this.apiFactoryClient.PostContentAsync($"{reIdentificationRelativeUrl}/impersonation", accessRequest);

        public async ValueTask<AccessRequest>
            PostImpersonationContextGenerateTokensAsync(Guid impersonationContextId) =>
            await this.apiFactoryClient.PostContentAsync<Guid, AccessRequest>(
                $"{reIdentificationRelativeUrl}/generatetokens",
                impersonationContextId);

        public async ValueTask<byte[]> GetCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId, string reason)
        {
            byte[] fileContent = await this.apiFactoryClient.GetContentByteArrayAsync($"{reIdentificationRelativeUrl}/" +
                $"csvreidentification/{csvIdentificationRequestId}/{reason}");

            return fileContent;
        }
    }
}
