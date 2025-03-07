// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;

namespace ISL.ReIdentification.Core.Services.Coordinations.Identifications
{
    public interface IIdentificationCoordinationService
    {
        ValueTask<AccessRequest> ProcessIdentificationRequestsAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> PersistsCsvIdentificationRequestAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> ProcessCsvIdentificationRequestAsync(Guid csvIdentificationRequestId, string reason);
        ValueTask<AccessRequest> PersistsImpersonationContextAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> ProcessImpersonationContextRequestAsync(string container, string filepath);
        ValueTask<AccessRequest> ExpireRenewImpersonationContextTokensAsync(Guid impersonationContextId);
        ValueTask ImpersonationContextApprovalAsync(Guid impersonationContextId, bool isApproved);
    }
}
