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
        ValueTask<AccessRequest> PersistImpersonationContextRequestAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> PersistCsvIdentificationRequestAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> RetrieveCsvIdentificationRequestAsync(Guid csvIdentificationRequestId);
    }
}
