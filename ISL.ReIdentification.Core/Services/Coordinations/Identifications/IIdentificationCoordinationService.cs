// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Identifications
{
    public interface IIdentificationCoordinationService
    {
        ValueTask<AccessRequest> ProcessImpersonationContextRequestAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> ProcessCsvIdentificationRequestAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> ReIdentifyCsvIdentificationRequestAsync(AccessRequest accessRequest);
        ValueTask<AccessRequest> ProcessIdentificationRequestsAsync(AccessRequest accessRequest);
    }
}
