// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial interface IReIdentificationStorageBroker
    {
        ValueTask<ImpersonationContext> InsertImpersonationContextAsync(ImpersonationContext impersonationContext);
        ValueTask<IQueryable<ImpersonationContext>> SelectAllImpersonationContextsAsync();
        ValueTask<ImpersonationContext> SelectImpersonationContextByIdAsync(Guid impersonationContextId);
        ValueTask<ImpersonationContext> UpdateImpersonationContextAsync(ImpersonationContext impersonationContext);
        ValueTask<ImpersonationContext> DeleteImpersonationContextAsync(ImpersonationContext impersonationContext);
    }
}
