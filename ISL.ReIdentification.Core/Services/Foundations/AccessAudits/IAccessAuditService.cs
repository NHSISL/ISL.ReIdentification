// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;

namespace ISL.ReIdentification.Core.Services.Foundations.AccessAudits
{
    public interface IAccessAuditService
    {
        ValueTask<AccessAudit> AddAccessAuditAsync(AccessAudit accessAudit);
        ValueTask<IQueryable<AccessAudit>> RetrieveAllAccessAuditsAsync();
        ValueTask<AccessAudit> RetrieveAccessAuditByIdAsync(Guid accessAuditId);
        ValueTask<AccessAudit> ModifyAccessAuditAsync(AccessAudit lookup);
        ValueTask<AccessAudit> RemoveAccessAuditByIdAsync(Guid accessAuditId);
    }
}
