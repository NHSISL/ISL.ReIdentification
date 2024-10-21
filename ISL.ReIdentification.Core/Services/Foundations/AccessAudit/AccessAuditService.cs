// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;

namespace ISL.ReIdentification.Core.Services.Foundations.AccessAudits
{
    public partial class AccessAuditService : IAccessAuditService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public AccessAuditService(
            IReIdentificationStorageBroker reIdentificationStorageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.reIdentificationStorageBroker = reIdentificationStorageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<AccessAudit> AddAccessAuditAsync(AccessAudit accessAudit) =>
            TryCatch(async () =>
            {
                await ValidateAccessAuditOnAddAsync(accessAudit);

                return await this.reIdentificationStorageBroker.InsertAccessAuditAsync(accessAudit);
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
                await ValidateAccessAuditOnModifyAsync(accessAudit);

                var maybeAccessAudit = await this.reIdentificationStorageBroker
                    .SelectAccessAuditByIdAsync(accessAudit.Id);

                await ValidateStorageAccessAuditAsync(maybeAccessAudit, accessAudit.Id);
                await ValidateAgainstStorageAccessAuditOnModifyAsync(accessAudit, maybeAccessAudit);

                return await this.reIdentificationStorageBroker.UpdateAccessAuditAsync(accessAudit);
            });

        public ValueTask<AccessAudit> RemoveAccessAuditByIdAsync(Guid accessAuditId) =>
        TryCatch(async () =>
        {
            await ValidateAccessAuditOnRemoveById(accessAuditId);

            var maybeAccessAudit = await this.reIdentificationStorageBroker
                .SelectAccessAuditByIdAsync(accessAuditId);

            await ValidateStorageAccessAuditAsync(maybeAccessAudit, accessAuditId);

            return await this.reIdentificationStorageBroker.DeleteAccessAuditAsync(maybeAccessAudit);
        });
    }
}
