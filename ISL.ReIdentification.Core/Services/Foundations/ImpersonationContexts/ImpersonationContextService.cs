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
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;

namespace ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts
{
    public partial class ImpersonationContextService : IImpersonationContextService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public ImpersonationContextService(
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

        public ValueTask<ImpersonationContext> AddImpersonationContextAsync(
            ImpersonationContext impersonationContext) =>
        TryCatch(async () =>
        {
            ImpersonationContext impersonationContextWithAddAuditApplied = 
                await ApplyAddAuditAsync(impersonationContext);
                    await ValidateImpersonationContextOnAddAsync(
                        impersonationContextWithAddAuditApplied);

                return await this.reIdentificationStorageBroker.InsertImpersonationContextAsync(
                    impersonationContextWithAddAuditApplied);
            });

        public ValueTask<ImpersonationContext> RetrieveImpersonationContextByIdAsync(Guid impersonationContextId) =>
            TryCatch(async () =>
            {
                ValidateImpersonationContextId(impersonationContextId);

                ImpersonationContext maybeImpersonationContext =
                    await this.reIdentificationStorageBroker.SelectImpersonationContextByIdAsync(impersonationContextId);

                ValidateStorageImpersonationContext(maybeImpersonationContext, impersonationContextId);

                return maybeImpersonationContext;
            });

        public ValueTask<IQueryable<ImpersonationContext>> RetrieveAllImpersonationContextsAsync() =>
            TryCatch(this.reIdentificationStorageBroker.SelectAllImpersonationContextsAsync);

        public ValueTask<ImpersonationContext> ModifyImpersonationContextAsync(
            ImpersonationContext impersonationContext) =>
        TryCatch(async () =>
        {
            ImpersonationContext impersonationContextWithModifyAuditApplied = 
            await ApplyModifyAuditAsync(impersonationContext);
            
            await ValidateImpersonationContextOnModifyAsync(impersonationContextWithModifyAuditApplied);

            ImpersonationContext maybeImpersonationContext = 
                await this.reIdentificationStorageBroker.
                    SelectImpersonationContextByIdAsync(impersonationContextWithModifyAuditApplied.Id);
            
            ValidateStorageImpersonationContext(
                maybeImpersonationContext, 
                impersonationContextWithModifyAuditApplied.Id);

            ValidateAgainstStorageImpersonationContextOnModify(
                inputImpersonationContext: impersonationContextWithModifyAuditApplied,
                storageImpersonationContext: maybeImpersonationContext);

            return await this.reIdentificationStorageBroker.UpdateImpersonationContextAsync(
                impersonationContextWithModifyAuditApplied);
        });

        public ValueTask<ImpersonationContext> RemoveImpersonationContextByIdAsync(Guid impersonationContextId) =>
            TryCatch(async () =>
            {
                ValidateImpersonationContextId(impersonationContextId);

                ImpersonationContext maybeImpersonationContext =
                    await this.reIdentificationStorageBroker.SelectImpersonationContextByIdAsync(impersonationContextId);

                ValidateStorageImpersonationContext(maybeImpersonationContext, impersonationContextId);

                return await this.reIdentificationStorageBroker.DeleteImpersonationContextAsync(maybeImpersonationContext);
            });

        virtual internal async ValueTask<ImpersonationContext> ApplyAddAuditAsync(
            ImpersonationContext impersonationContext)
        {
            ValidateImpersonationContextIsNotNull(impersonationContext);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            impersonationContext.CreatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            impersonationContext.CreatedDate = auditDateTimeOffset;
            impersonationContext.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            impersonationContext.UpdatedDate = auditDateTimeOffset;

            return impersonationContext;
        }

        virtual internal async ValueTask<ImpersonationContext> ApplyModifyAuditAsync(
            ImpersonationContext impersonationContext)
        {
            ValidateImpersonationContextIsNotNull(impersonationContext);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            impersonationContext.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            impersonationContext.UpdatedDate = auditDateTimeOffset;

            return impersonationContext;
        }
    }
}