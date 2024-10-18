// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
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
            Validate((Rule: IsInvalid(accessRequest.IdentificationRequest),
                Parameter: nameof(AccessRequest.IdentificationRequest)));
        }

        private static void ValidateOnPersistsCsvIdentificationRequest(AccessRequest accessRequest)
        {
            ValidateAccessRequestIsNotNull(accessRequest);
            Validate((Rule: IsInvalid(accessRequest.CsvIdentificationRequest),
                Parameter: nameof(AccessRequest.CsvIdentificationRequest)));
        }

        private static void ValidateOnPersistsImpersonationContext(AccessRequest accessRequest)
        {
            ValidateAccessRequestIsNotNull(accessRequest);
            Validate((Rule: IsInvalid(accessRequest.ImpersonationContext),
                Parameter: nameof(AccessRequest.ImpersonationContext)));
        }

        private static void ValidateAccessRequestIsNotNull(AccessRequest accessRequest)
        {
            if (accessRequest is null)
            {
                throw new NullAccessRequestException("Access request is null.");
            }
        }

        public static void ValidateCsvIdentificationRequestId(Guid csvIdentificationRequestId) =>
            Validate((Rule: IsInvalid(csvIdentificationRequestId), Parameter: nameof(CsvIdentificationRequest.Id)));

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        private static dynamic IsInvalid(Object @object) => new
        {
            Condition = @object is null,
            Message = "Object is invalid"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidIdentificationCoordinationException =
                new InvalidIdentificationCoordinationException(
                    message: "Invalid identification coordination exception. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidIdentificationCoordinationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidIdentificationCoordinationException.ThrowIfContainsErrors();
        }
    }
}
