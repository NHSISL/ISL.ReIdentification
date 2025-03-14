// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.Audit;

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string auditsRelativeUrl = "api/audits";

        public async ValueTask<Audit> PostAuditAsync(Audit audit) =>
            await this.apiFactoryClient.PostContentAsync(auditsRelativeUrl, audit);

        public async ValueTask<List<Audit>> GetAllAuditsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<Audit>>($"{auditsRelativeUrl}/");

        public async ValueTask<List<Audit>> GetSpecificAuditByIdAsync(Guid auditId) =>
            await this.apiFactoryClient.GetContentAsync<List<Audit>>(
                $"{auditsRelativeUrl}?$filter=Id eq {auditId}");

        public async ValueTask<Audit> GetAuditByIdAsync(Guid auditId) =>
            await this.apiFactoryClient
                .GetContentAsync<Audit>($"{auditsRelativeUrl}/{auditId}");

        public async ValueTask<Audit> DeleteAuditByIdAsync(Guid auditId) =>
            await this.apiFactoryClient
                .DeleteContentAsync<Audit>($"{auditsRelativeUrl}/{auditId}");

        public async ValueTask<Audit> PutAuditAsync(Audit audit) =>
            await this.apiFactoryClient.PutContentAsync(auditsRelativeUrl, audit);
    }
}