// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Identifications
{
    public interface IIdentificationOrchestrationService
    {
        ValueTask<IdentificationRequest> ProcessIdentificationRequestAsync(
            IdentificationRequest identificationRequest);

        ValueTask RemoveDocumentByFileNameAsync(string fileName, string container);
    }
}
