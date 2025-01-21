// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using Microsoft.EntityFrameworkCore;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
        public DbSet<Audit> Audits { get; set; }

        public async ValueTask<Audit> InsertAuditAsync(Audit audit) =>
            await InsertAsync(audit);

        public async ValueTask<IQueryable<Audit>> SelectAllAuditsAsync() =>
            await SelectAllAsync<Audit>();

        public async ValueTask<Audit> SelectAuditByIdAsync(Guid auditId) =>
            await SelectAsync<Audit>(auditId);

        public async ValueTask<Audit> SelectAuditByAuditDetailAsync(string auditType) =>
           await SelectAsync<Audit>(auditType);

        public async ValueTask<Audit> UpdateAuditAsync(Audit audit) =>
            await UpdateAsync(audit);

        public async ValueTask<Audit> DeleteAuditAsync(Audit audit) =>
            await DeleteAsync(audit);
    }
}