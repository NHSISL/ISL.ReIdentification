// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;

namespace ISL.ReIdentification.Core.Services.Foundations.Lookups
{
    public partial class LookupService : ILookupService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public LookupService(
            IReIdentificationStorageBroker reIdentificationStorageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.reIdentificationStorageBroker = reIdentificationStorageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Lookup> AddLookupAsync(Lookup lookup) =>
            TryCatch(async () =>
            {
                await ValidateLookupOnAddAsync(lookup);

                return await this.reIdentificationStorageBroker.InsertLookupAsync(lookup);
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
                await ValidateLookupOnModifyAsync(lookup);

                Lookup maybeLookup =
                    await this.reIdentificationStorageBroker.SelectLookupByIdAsync(lookup.Id);

                ValidateStorageLookup(maybeLookup, lookup.Id);
                ValidateAgainstStorageLookupOnModify(inputLookup: lookup, storageLookup: maybeLookup);

                return await this.reIdentificationStorageBroker.UpdateLookupAsync(lookup);
            });

        public ValueTask<Lookup> RemoveLookupByIdAsync(Guid lookupId) =>
            TryCatch(async () =>
            {
                ValidateLookupId(lookupId);

                Lookup maybeLookup = await this.reIdentificationStorageBroker
                    .SelectLookupByIdAsync(lookupId);

                ValidateStorageLookup(maybeLookup, lookupId);

                return await this.reIdentificationStorageBroker.DeleteLookupAsync(maybeLookup);
            });
    }
}