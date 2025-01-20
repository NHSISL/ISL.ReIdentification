// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.Audits;

namespace ISL.ReIdentification.Core.Services.Foundations.Audits
{
    public partial class AuditService : IAuditService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public AuditService(
            IReIdentificationStorageBroker reIdentificationStorageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.reIdentificationStorageBroker = reIdentificationStorageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Audit> AddAuditAsync(Audit audit) =>
            TryCatch(async () =>
            {
                await ValidateAuditOnAddAsync(audit);

                return await this.reIdentificationStorageBroker.InsertAuditAsync(audit);
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
                await ValidateAuditOnModifyAsync(audit);

                var maybeAudit = await this.reIdentificationStorageBroker
                    .SelectAuditByIdAsync(audit.Id);

                await ValidateStorageAuditAsync(maybeAudit, audit.Id);
                await ValidateAgainstStorageAuditOnModifyAsync(audit, maybeAudit);

                return await this.reIdentificationStorageBroker.UpdateAuditAsync(audit);
            });

        public ValueTask<Audit> RemoveAuditByIdAsync(Guid auditId) =>
        TryCatch(async () =>
        {
            await ValidateAuditOnRemoveById(auditId);

            var maybeAudit = await this.reIdentificationStorageBroker
                .SelectAuditByIdAsync(auditId);

            await ValidateStorageAuditAsync(maybeAudit, auditId);

            return await this.reIdentificationStorageBroker.DeleteAuditAsync(maybeAudit);
        });
    }
}
