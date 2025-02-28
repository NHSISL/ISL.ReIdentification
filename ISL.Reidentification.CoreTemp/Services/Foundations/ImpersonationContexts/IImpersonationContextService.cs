// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;

namespace ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts
{
    public interface IImpersonationContextService
    {
        ValueTask<ImpersonationContext> AddImpersonationContextAsync(ImpersonationContext impersonationContext);
        ValueTask<ImpersonationContext> RetrieveImpersonationContextByIdAsync(Guid impersonationContextId);
        ValueTask<IQueryable<ImpersonationContext>> RetrieveAllImpersonationContextsAsync();
        ValueTask<ImpersonationContext> ModifyImpersonationContextAsync(ImpersonationContext impersonationContext);
        ValueTask<ImpersonationContext> RemoveImpersonationContextByIdAsync(Guid impersonationContextId);
    }
}
