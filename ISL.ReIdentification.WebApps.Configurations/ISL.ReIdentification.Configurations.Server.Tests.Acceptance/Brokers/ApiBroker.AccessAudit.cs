// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.AccessAudits;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string accessAuditsRelativeUrl = "api/accessAudits";

        public async ValueTask<AccessAudit> PostAccessAuditAsync(AccessAudit accessAudit) =>
            await this.apiFactoryClient.PostContentAsync(accessAuditsRelativeUrl, accessAudit);

        public async ValueTask<List<AccessAudit>> GetAllAccessAuditsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<AccessAudit>>($"{accessAuditsRelativeUrl}/");

        public async ValueTask<List<AccessAudit>> GetSpecificAccessAuditByIdAsync(Guid lookupId) =>
            await this.apiFactoryClient.GetContentAsync<List<AccessAudit>>(
                $"{accessAuditsRelativeUrl}?$filter=Id eq {lookupId}");

        public async ValueTask<AccessAudit> GetAccessAuditByIdAsync(Guid accessAuditId) =>
            await this.apiFactoryClient
                .GetContentAsync<AccessAudit>($"{accessAuditsRelativeUrl}/{accessAuditId}");

        public async ValueTask<AccessAudit> DeleteAccessAuditByIdAsync(Guid accessAuditId) =>
            await this.apiFactoryClient
                .DeleteContentAsync<AccessAudit>($"{accessAuditsRelativeUrl}/{accessAuditId}");

        public async ValueTask<AccessAudit> PutAccessAuditAsync(AccessAudit accessAudit) =>
            await this.apiFactoryClient.PutContentAsync(accessAuditsRelativeUrl, accessAudit);
    }
}
