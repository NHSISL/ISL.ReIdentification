// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using Microsoft.EntityFrameworkCore;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
        public DbSet<AccessAudit> AccessAudits { get; set; }

        public async ValueTask<AccessAudit> InsertAccessAuditAsync(AccessAudit accessAudit) =>
            await InsertAsync(accessAudit);

        public async ValueTask<IQueryable<AccessAudit>> SelectAllAccessAuditsAsync() =>
            await SelectAllAsync<AccessAudit>();

        public async ValueTask<AccessAudit> SelectAccessAuditByIdAsync(Guid accessAuditId) =>
            await SelectAsync<AccessAudit>(accessAuditId);

        public async ValueTask<AccessAudit> UpdateAccessAuditAsync(AccessAudit accessAudit) =>
            await UpdateAsync(accessAudit);

        public async ValueTask<AccessAudit> DeleteAccessAuditAsync(AccessAudit accessAudit) =>
            await DeleteAsync(accessAudit);
    }
}