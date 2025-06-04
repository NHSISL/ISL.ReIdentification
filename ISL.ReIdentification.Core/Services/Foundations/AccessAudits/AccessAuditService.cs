// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Securities;

namespace ISL.ReIdentification.Core.Services.Foundations.AccessAudits
{
    public partial class AccessAuditService : IAccessAuditService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public AccessAuditService(
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

        public ValueTask BulkAddAccessAuditAsync(List<AccessAudit> accessAudits) =>
        TryCatch(async () =>
        {
            List<AccessAudit> accessAuditsWithAddAuditApplied = await ApplyBulkAddAuditAsync(accessAudits);
            await ValidateAccessAuditOnBulkAddAsync(accessAuditsWithAddAuditApplied);
            await this.reIdentificationStorageBroker.InsertBulkAccessAuditAsync(accessAuditsWithAddAuditApplied);
        });

        public ValueTask<AccessAudit> AddAccessAuditAsync(AccessAudit accessAudit) =>
        TryCatch(async () =>
        {
            AccessAudit accessAuditWithAddAuditApplied = await ApplyAddAuditAsync(accessAudit);
            await ValidateAccessAuditOnAddAsync(accessAuditWithAddAuditApplied);

            return await this.reIdentificationStorageBroker.InsertAccessAuditAsync(accessAuditWithAddAuditApplied);
        });

        public ValueTask<IQueryable<AccessAudit>> RetrieveAllAccessAuditsAsync() =>
            TryCatch(this.reIdentificationStorageBroker.SelectAllAccessAuditsAsync);

        public ValueTask<AccessAudit> RetrieveAccessAuditByIdAsync(Guid accessAuditId) =>
            TryCatch(async () =>
            {
                await ValidateAccessAuditOnRetrieveById(accessAuditId);

                var maybeAccessAudit = await this.reIdentificationStorageBroker
                    .SelectAccessAuditByIdAsync(accessAuditId);

                await ValidateStorageAccessAuditAsync(maybeAccessAudit, accessAuditId);

                return maybeAccessAudit;
            });

        public ValueTask<AccessAudit> ModifyAccessAuditAsync(AccessAudit accessAudit) =>
            TryCatch(async () =>
            {
                AccessAudit accessAuditWithModifyAuditApplied = await ApplyModifyAuditAsync(accessAudit);
                await ValidateAccessAuditOnModifyAsync(accessAuditWithModifyAuditApplied);

                var maybeAccessAudit = await this.reIdentificationStorageBroker
                    .SelectAccessAuditByIdAsync(accessAuditWithModifyAuditApplied.Id);

                await ValidateStorageAccessAuditAsync(maybeAccessAudit, accessAuditWithModifyAuditApplied.Id);

                await ValidateAgainstStorageAccessAuditOnModifyAsync(
                    accessAuditWithModifyAuditApplied,
                    maybeAccessAudit);

                return await this.reIdentificationStorageBroker
                    .UpdateAccessAuditAsync(accessAuditWithModifyAuditApplied);
            });

        public ValueTask<AccessAudit> RemoveAccessAuditByIdAsync(Guid accessAuditId) =>
            TryCatch(async () =>
            {
                await ValidateAccessAuditOnRemoveById(accessAuditId);

                AccessAudit maybeAccessAudit = 
                    await this.reIdentificationStorageBroker.SelectAccessAuditByIdAsync(accessAuditId);

                await ValidateStorageAccessAuditAsync(maybeAccessAudit, accessAuditId);
                AccessAudit accessAuditWithDeleteAuditApplied = await ApplyDeleteAuditAsync(maybeAccessAudit);

                AccessAudit updatedAccessAudit =
                    await this.reIdentificationStorageBroker.UpdateAccessAuditAsync(accessAuditWithDeleteAuditApplied);

                await ValidateAgainstStorageAccessAuditOnDeleteAsync(
                    updatedAccessAudit,
                    accessAuditWithDeleteAuditApplied);

                return await this.reIdentificationStorageBroker.DeleteAccessAuditAsync(updatedAccessAudit);
            });

        virtual internal async ValueTask<AccessAudit> ApplyAddAuditAsync(AccessAudit accessAudit)
        {
            ValidateAccessAuditIsNotNull(accessAudit);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            accessAudit.CreatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            accessAudit.CreatedDate = auditDateTimeOffset;
            accessAudit.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            accessAudit.UpdatedDate = auditDateTimeOffset;

            return accessAudit;
        }

        virtual internal async ValueTask<List<AccessAudit>> ApplyBulkAddAuditAsync(List<AccessAudit> accessAudits)
        {
            ValidateAccessAuditIsNotNull(accessAudits);
            var currentDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            EntraUser currentUser = await this.securityBroker.GetCurrentUserAsync();

            foreach (var accessAudit in accessAudits)
            {
                accessAudit.CreatedBy = currentUser?.EntraUserId;
                accessAudit.CreatedDate = currentDateTimeOffset;
                accessAudit.UpdatedBy = currentUser?.EntraUserId;
                accessAudit.UpdatedDate = currentDateTimeOffset;
            }

            return accessAudits;
        }

        virtual internal async ValueTask<AccessAudit> ApplyModifyAuditAsync(AccessAudit accessAudit)
        {
            ValidateAccessAuditIsNotNull(accessAudit);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            accessAudit.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            accessAudit.UpdatedDate = auditDateTimeOffset;

            return accessAudit;
        }

        virtual internal async ValueTask<AccessAudit> ApplyDeleteAuditAsync(AccessAudit accessAudit)
        {
            ValidateAccessAuditIsNotNull(accessAudit);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            accessAudit.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            accessAudit.UpdatedDate = auditDateTimeOffset;
            return accessAudit;
        }
    }
}
