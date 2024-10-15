// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Identifications
{
    public partial class IdentificationCoordinationService : IIdentificationCoordinationService
    {
        private static void ValidateOnProcessIdentificationRequests(AccessRequest accessRequest)
        {
            ValidateAccessRequestIsNotNull(accessRequest);
            ValidateIdentificationRequestIsNotNull(accessRequest.IdentificationRequest);
        }

        private static void ValidateOnPersistsCsvIdentificationRequest(AccessRequest accessRequest)
        {
            ValidateAccessRequestIsNotNull(accessRequest);
            ValidateCsvIdentificationRequestIsNotNull(accessRequest.CsvIdentificationRequest);
        }

        private static void ValidateOnPersistsImpersonationContext(AccessRequest accessRequest)
        {
            ValidateAccessRequestIsNotNull(accessRequest);
            ValidateImpersonationContextIsNotNull(accessRequest.ImpersonationContext);
        }

        private static void ValidateAccessRequestIsNotNull(AccessRequest accessRequest)
        {
            if (accessRequest is null)
            {
                throw new NullAccessRequestException("Access request is null.");
            }
        }

        private static void ValidateIdentificationRequestIsNotNull(IdentificationRequest identificationRequest)
        {
            if (identificationRequest is null)
            {
                throw new NullIdentificationRequestException("Identification request is null.");
            }
        }

        private static void ValidateCsvIdentificationRequestIsNotNull(CsvIdentificationRequest csvIdentificationRequest)
        {
            if (csvIdentificationRequest is null)
            {
                throw new NullCsvIdentificationRequestException("CSV identification request is null.");
            }
        }

        private static void ValidateImpersonationContextIsNotNull(ImpersonationContext impersonationContext)
        {
            if (impersonationContext is null)
            {
                throw new NullImpersonationContextException("Impersonation context is null.");
            }
        }
    }
}
