﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using ISL.ReIdentification.Core.Models.Foundations.DelegatedAccesses;
using Microsoft.EntityFrameworkCore;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
       public DbSet<DelegatedAccess> DelegatedAccesses { get; set; }

        public async ValueTask<DelegatedAccess> InsertDelegatedAccessAsync(DelegatedAccess delegatedAccess) =>
            await InsertAsync(delegatedAccess);

        public async ValueTask<IQueryable<DelegatedAccess>> SelectAllDelegatedAccessesAsync() =>
            await SelectAllAsync<DelegatedAccess>();

        public async ValueTask<DelegatedAccess> SelectDelegatedAccessByIdAsync(Guid delegateAccessId) =>
            await SelectAsync<DelegatedAccess>(delegateAccessId);

        public async ValueTask<DelegatedAccess> UpdateDelegatedAccessAsync(DelegatedAccess delegatedAccess) =>
            await UpdateAsync(delegatedAccess);
            
        public async ValueTask<DelegatedAccess> DeleteDelegatedAccessAsync(DelegatedAccess delegatedAccess) =>
            await DeleteAsync(delegatedAccess);
    }
}