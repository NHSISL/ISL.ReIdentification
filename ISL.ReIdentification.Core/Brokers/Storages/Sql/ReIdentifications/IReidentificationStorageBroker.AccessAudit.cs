// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial interface IReIdentificationStorageBroker
    {
        ValueTask<AccessAudit> InsertAccessAuditAsync(AccessAudit accessAudit);
        ValueTask<IQueryable<AccessAudit>> SelectAllAccessAuditsAsync();
        ValueTask<AccessAudit> SelectAccessAuditByIdAsync(Guid accessAuditId);
        ValueTask<AccessAudit> UpdateAccessAuditAsync(AccessAudit accessAudit);
        ValueTask<AccessAudit> DeleteAccessAuditAsync(AccessAudit accessAudit);
    }
}
