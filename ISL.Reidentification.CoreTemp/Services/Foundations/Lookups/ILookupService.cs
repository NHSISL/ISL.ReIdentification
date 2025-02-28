// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;

namespace ISL.ReIdentification.Core.Services.Foundations.Lookups
{
    public interface ILookupService
    {
        ValueTask<Lookup> AddLookupAsync(Lookup lookup);
        ValueTask<IQueryable<Lookup>> RetrieveAllLookupsAsync();
        ValueTask<Lookup> RetrieveLookupByIdAsync(Guid lookupId);
        ValueTask<Lookup> ModifyLookupAsync(Lookup lookup);
        ValueTask<Lookup> RemoveLookupByIdAsync(Guid lookupId);
    }
}