// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.Audits;

namespace ISL.ReIdentification.Core.Services.Foundations.Audits
{
    public partial class AuditService : IAuditService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public AuditService(
            IReIdentificationStorageBroker reIdentificationStorageBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityBroker securityBroker,
            ILoggingBroker loggingBroker)
        {
            this.reIdentificationStorageBroker = reIdentificationStorageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityBroker = securityBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Audit> AddAuditAsync(Audit audit) =>
            TryCatch(async () =>
            {
                Audit auditWithAddAuditApplied = await ApplyAddAuditAsync(audit);
                await ValidateAuditOnAddAsync(auditWithAddAuditApplied);

                return await this.reIdentificationStorageBroker.InsertAuditAsync(auditWithAddAuditApplied);
            });

        public ValueTask<IQueryable<Audit>> RetrieveAllAuditsAsync() =>
            TryCatch(this.reIdentificationStorageBroker.SelectAllAuditsAsync);

        public ValueTask<Audit> RetrieveAuditByIdAsync(Guid auditId) =>
            TryCatch(async () =>
            {
                await ValidateAuditOnRetrieveById(auditId);

                var maybeAudit = await this.reIdentificationStorageBroker
                    .SelectAuditByIdAsync(auditId);

                await ValidateStorageAuditAsync(maybeAudit, auditId);

                return maybeAudit;
            });

        public ValueTask<Audit> ModifyAuditAsync(Audit audit) =>
            TryCatch(async () =>
            {
                Audit auditWithModifyAuditApplied = await ApplyModifyAuditAsync(audit);
                await ValidateAuditOnModifyAsync(auditWithModifyAuditApplied);
                var maybeAudit = await this.reIdentificationStorageBroker.SelectAuditByIdAsync(audit.Id);
                await ValidateStorageAuditAsync(maybeAudit, audit.Id);
                await ValidateAgainstStorageAuditOnModifyAsync(auditWithModifyAuditApplied, maybeAudit);

                return await this.reIdentificationStorageBroker.UpdateAuditAsync(auditWithModifyAuditApplied);
            });

        public ValueTask<Audit> RemoveAuditByIdAsync(Guid auditId) =>
        TryCatch(async () =>
        {
            await ValidateAuditOnRemoveByIdAsync(auditId);
            Audit maybeAudit = await this.reIdentificationStorageBroker.SelectAuditByIdAsync(auditId);
            await ValidateStorageAuditAsync(maybeAudit, auditId);
            Audit auditWithDeleteAuditApplied = await ApplyDeleteAuditAsync(maybeAudit);

            Audit updatedAudit =
                await this.reIdentificationStorageBroker.UpdateAuditAsync(auditWithDeleteAuditApplied);

            await ValidateAgainstStorageAuditOnDeleteAsync(
                audit: updatedAudit,
                maybeAudit: auditWithDeleteAuditApplied);

            return await this.reIdentificationStorageBroker.DeleteAuditAsync(updatedAudit);
        });

        virtual internal async ValueTask<Audit> ApplyAddAuditAsync(Audit audit)
        {
            ValidateAuditIsNotNull(audit);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            audit.CreatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            audit.CreatedDate = auditDateTimeOffset;
            audit.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            audit.UpdatedDate = auditDateTimeOffset;

            return audit;
        }

        virtual internal async ValueTask<Audit> ApplyModifyAuditAsync(Audit audit)
        {
            ValidateAuditIsNotNull(audit);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            audit.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            audit.UpdatedDate = auditDateTimeOffset;

            return audit;
        }

        virtual internal async ValueTask<Audit> ApplyDeleteAuditAsync(Audit audit)
        {
            ValidateAuditIsNotNull(audit);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            audit.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            audit.UpdatedDate = auditDateTimeOffset;

            return audit;
        }
    }
}
