// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Identifications
{
    public interface IIdentificationOrchestrationService
    {
        ValueTask<IdentificationRequest> ProcessIdentificationRequestAsync(
            IdentificationRequest identificationRequest);

        ValueTask AddDocumentAsync(Stream input, string fileName, string container);
        ValueTask RetrieveDocumentByFileNameAsync(Stream output, string fileName, string container);
        ValueTask RemoveDocumentByFileNameAsync(string filename, string container);
        ValueTask<AccessRequest> ExpireRenewImpersonationContextTokensAsync(AccessRequest accessRequest);
    }
}
