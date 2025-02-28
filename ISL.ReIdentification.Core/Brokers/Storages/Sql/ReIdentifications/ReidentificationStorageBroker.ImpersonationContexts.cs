// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using Microsoft.EntityFrameworkCore;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
        public DbSet<ImpersonationContext> ImpersonationContexts { get; set; }

        public async ValueTask<ImpersonationContext> InsertImpersonationContextAsync(
            ImpersonationContext impersonationContext) =>
                await InsertAsync(impersonationContext);

        public async ValueTask<IQueryable<ImpersonationContext>> SelectAllImpersonationContextsAsync() =>
            await SelectAllAsync<ImpersonationContext>();

        public async ValueTask<ImpersonationContext> SelectImpersonationContextByIdAsync(Guid delegateAccessId) =>
            await SelectAsync<ImpersonationContext>(delegateAccessId);

        public async ValueTask<ImpersonationContext> UpdateImpersonationContextAsync(
            ImpersonationContext impersonationContext) =>
                await UpdateAsync(impersonationContext);

        public async ValueTask<ImpersonationContext> DeleteImpersonationContextAsync(
            ImpersonationContext impersonationContext) =>
                await DeleteAsync(impersonationContext);
    }
}