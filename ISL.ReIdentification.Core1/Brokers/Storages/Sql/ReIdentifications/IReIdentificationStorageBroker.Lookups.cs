// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial interface IReIdentificationStorageBroker
    {
        ValueTask<Lookup> InsertLookupAsync(Lookup lookup);
        ValueTask<IQueryable<Lookup>> SelectAllLookupsAsync();
        ValueTask<Lookup> SelectLookupByIdAsync(Guid lookupId);
        ValueTask<Lookup> UpdateLookupAsync(Lookup lookup);
        ValueTask<Lookup> DeleteLookupAsync(Lookup lookup);
    }
}
