// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;

namespace ISL.ReIdentification.Core.Services.Foundations.ReIdentifications
{
    public interface IReIdentificationService
    {
        ValueTask<IdentificationRequest> ProcessReIdentificationRequest(
            IdentificationRequest identificationRequests);
    }
}
