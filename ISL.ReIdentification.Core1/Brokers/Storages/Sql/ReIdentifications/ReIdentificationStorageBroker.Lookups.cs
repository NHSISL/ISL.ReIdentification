// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using Microsoft.EntityFrameworkCore;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker : EFxceptionsContext, IReIdentificationStorageBroker
    {
        public DbSet<Lookup> Lookups { get; set; }

        public async ValueTask<Lookup> InsertLookupAsync(Lookup lookup) =>
            await InsertAsync(lookup);

        public async ValueTask<IQueryable<Lookup>> SelectAllLookupsAsync() =>
            await SelectAllAsync<Lookup>();

        public async ValueTask<Lookup> SelectLookupByIdAsync(Guid lookupId) =>
            await SelectAsync<Lookup>(lookupId);

        public async ValueTask<Lookup> UpdateLookupAsync(Lookup lookup) =>
            await UpdateAsync(lookup);

        public async ValueTask<Lookup> DeleteLookupAsync(Lookup lookup) =>
            await DeleteAsync(lookup);
    }
}