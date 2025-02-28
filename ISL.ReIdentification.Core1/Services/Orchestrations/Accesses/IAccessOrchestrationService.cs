// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Accesses
{
    public interface IAccessOrchestrationService
    {
        ValueTask<AccessRequest> ProcessImpersonationContextRequestAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> ValidateAccessForIdentificationRequestAsync(AccessRequest accessRequest);
    }
}
