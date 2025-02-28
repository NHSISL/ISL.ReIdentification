// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.Audits;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial interface IReIdentificationStorageBroker
    {
        ValueTask<Audit> InsertAuditAsync(Audit audit);
        ValueTask<IQueryable<Audit>> SelectAllAuditsAsync();
        ValueTask<Audit> SelectAuditByIdAsync(Guid auditId);
        ValueTask<Audit> UpdateAuditAsync(Audit audit);
        ValueTask<Audit> DeleteAuditAsync(Audit audit);
    }
}
