// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;

namespace ISL.ReIdentification.Core.Services.Foundations.Lookups
{
    public partial class LookupService : ILookupService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public LookupService(
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

        public ValueTask<Lookup> AddLookupAsync(Lookup lookup) =>
        TryCatch(async () =>
        {
            Lookup lookupWithAddAuditApplied = await ApplyAddAuditAsync(lookup);
            await ValidateLookupOnAddAsync(lookupWithAddAuditApplied);

            return await this.reIdentificationStorageBroker.InsertLookupAsync(lookupWithAddAuditApplied);
        });

        public ValueTask<IQueryable<Lookup>> RetrieveAllLookupsAsync() =>
            TryCatch(this.reIdentificationStorageBroker.SelectAllLookupsAsync);

        public ValueTask<Lookup> RetrieveLookupByIdAsync(Guid lookupId) =>
            TryCatch(async () =>
            {
                ValidateLookupId(lookupId);

                Lookup maybeLookup = await this.reIdentificationStorageBroker
                    .SelectLookupByIdAsync(lookupId);

                ValidateStorageLookup(maybeLookup, lookupId);

                return maybeLookup;
            });

        public ValueTask<Lookup> ModifyLookupAsync(Lookup lookup) =>
            TryCatch(async () =>
            {
                Lookup lookupWithModifiedAuditApplied = await ApplyModifyAuditAsync(lookup);
                await ValidateLookupOnModifyAsync(lookupWithModifiedAuditApplied);

                Lookup maybeLookup =
                    await this.reIdentificationStorageBroker.SelectLookupByIdAsync(lookupWithModifiedAuditApplied.Id);

                ValidateStorageLookup(maybeLookup, lookupWithModifiedAuditApplied.Id);

                ValidateAgainstStorageLookupOnModify(
                    inputLookup: lookupWithModifiedAuditApplied,
                    storageLookup: maybeLookup);

                return await this.reIdentificationStorageBroker.UpdateLookupAsync(lookupWithModifiedAuditApplied);
            });

        public ValueTask<Lookup> RemoveLookupByIdAsync(Guid lookupId) =>
            TryCatch(async () =>
            {
                ValidateLookupId(lookupId);
                Lookup maybeLookup = await this.reIdentificationStorageBroker.SelectLookupByIdAsync(lookupId);
                ValidateStorageLookup(maybeLookup, lookupId);
                Lookup lookupWithDeleteAuditApplied = await ApplyDeleteAuditAsync(maybeLookup);
                
                Lookup updatedLookup =
                    await this.reIdentificationStorageBroker.UpdateLookupAsync(lookupWithDeleteAuditApplied);

                return await this.reIdentificationStorageBroker.DeleteLookupAsync(updatedLookup);
            });

        virtual internal async ValueTask<Lookup> ApplyAddAuditAsync(Lookup lookup)
        {
            ValidateLookupIsNotNull(lookup);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            lookup.CreatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            lookup.CreatedDate = auditDateTimeOffset;
            lookup.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            lookup.UpdatedDate = auditDateTimeOffset;

            return lookup;
        }

        virtual internal async ValueTask<Lookup> ApplyModifyAuditAsync(Lookup lookup)
        {
            ValidateLookupIsNotNull(lookup);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            lookup.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            lookup.UpdatedDate = auditDateTimeOffset;

            return lookup;
        }

        virtual internal async ValueTask<Lookup> ApplyDeleteAuditAsync(Lookup lookup)
        {
            ValidateLookupIsNotNull(lookup);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            lookup.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            lookup.UpdatedDate = auditDateTimeOffset;
            return lookup;
        }
    }
}