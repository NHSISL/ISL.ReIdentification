// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.Accesses;
using Microsoft.AspNetCore.Mvc;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string reIdentificationRelativeUrl = "api/reidentification";

        public async ValueTask<AccessRequest> PostImpersonationContextRequestAsync(AccessRequest accessRequest) =>
            await this.apiFactoryClient.PostContentAsync($"{reIdentificationRelativeUrl}/impersonation", accessRequest);

        public async ValueTask<ActionResult> GetCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId, string reason) =>
            await this.apiFactoryClient.GetContentAsync<FileContentResult>($"{reIdentificationRelativeUrl}/" +
                $"csvreidentification/{csvIdentificationRequestId}/{reason}");
    }
}
