// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Persists
{
    public interface IPersistanceOrchestrationService
    {
        ValueTask<AccessRequest> ProcessImpersonationContextRequestAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> ProcessCsvIdentificationRequestAsync(AccessRequest accessRequest);
    }
}
