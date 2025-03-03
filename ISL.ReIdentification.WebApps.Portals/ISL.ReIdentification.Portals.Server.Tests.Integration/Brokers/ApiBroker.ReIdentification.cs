// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.Accesses;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.ReIdentifications;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string reIdentificationRelativeUrl = "api/reidentification";

        public async ValueTask<AccessRequest> PostIdentificationRequestsAsync(AccessRequest accessRequest) =>
            await this.apiFactoryClient.PostContentAsync($"{reIdentificationRelativeUrl}", accessRequest);

        public async ValueTask<byte[]> PostCsvIdentificationRequestAsync(
            Guid csvIdentificationRequestId,
            string reason)
        {
            byte[] fileContent = await this.apiFactoryClient.GetContentByteArrayAsync(
                    $"{reIdentificationRelativeUrl}/csvreidentification/{csvIdentificationRequestId}/{reason}");

            return fileContent;
        }

        public async ValueTask<AccessRequest> PostImpersonationContextGenerateTokensAsync(Guid impersonationContextId)
        {
            return await this.apiFactoryClient.PostContentAsync<Guid, AccessRequest>(
                $"{reIdentificationRelativeUrl}/generatetokens",
                impersonationContextId);
        }

        public async ValueTask PostImpersonationContextApprovalAsync(ApprovalRequest approvalRequest) =>
            await this.apiFactoryClient.PostContentWithNoResponseAsync(
                $"{reIdentificationRelativeUrl}/impersonationcontextapproval",
                approvalRequest);
    }
}
