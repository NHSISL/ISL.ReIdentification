// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Persists
{
    public interface IPersistanceOrchestrationService
    {
        ValueTask<AccessRequest> PersistImpersonationContextAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> RetrieveImpersonationContextByIdAsync(Guid impersonationContextId);
        ValueTask<AccessRequest> PersistCsvIdentificationRequestAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> RetrieveCsvIdentificationRequestByIdAsync(Guid csvIdentificationRequestId);
    }
}
