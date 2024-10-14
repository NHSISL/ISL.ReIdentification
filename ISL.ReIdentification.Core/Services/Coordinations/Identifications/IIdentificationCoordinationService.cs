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
        ValueTask<AccessRequest> ProcessImpersonationContextRequestAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> PersistsCsvIdentificationRequestAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> ProcessCsvIdentificationRequestAsync(Guid csvIdentificationRequestId);
        ValueTask<AccessRequest> ReIdentifyCsvIdentificationRequestAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> ProcessIdentificationRequestsAsync(AccessRequest accessRequest);
    }
}
