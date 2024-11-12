﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.AccessAudits;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string AccessAuditsRelativeUrl = "api/accessAudits";

        public async ValueTask<AccessAudit> PostAccessAuditAsync(AccessAudit accessAudit) =>
            await this.apiFactoryClient.PostContentAsync(AccessAuditsRelativeUrl, accessAudit);

        public async ValueTask<List<AccessAudit>> GetAllAccessAuditsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<AccessAudit>>($"{AccessAuditsRelativeUrl}/");

        public async ValueTask<AccessAudit> GetAccessAuditByIdAsync(Guid accessAuditId) =>
            await this.apiFactoryClient
                .GetContentAsync<AccessAudit>($"{AccessAuditsRelativeUrl}/{accessAuditId}");

        public async ValueTask<AccessAudit> DeleteAccessAuditByIdAsync(Guid accessAuditId) =>
            await this.apiFactoryClient
                .DeleteContentAsync<AccessAudit>($"{AccessAuditsRelativeUrl}/{accessAuditId}");

        public async ValueTask<AccessAudit> PutAccessAuditAsync(AccessAudit accessAudit) =>
            await this.apiFactoryClient.PutContentAsync(AccessAuditsRelativeUrl, accessAudit);
    }
}
