// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationService
    {
        private static void ValidateOnRetrieveCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId)
        {
            Validate(
                (Rule: IsInvalid(csvIdentificationRequestId), Parameter: nameof(csvIdentificationRequestId)));
        }

        private static void ValidateAccessRequestIsNotNull(AccessRequest accessRequest)
        {
            if (accessRequest is null)
            {
                throw new NullAccessRequestException("Access request is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidArgumentPersistanceOrchestrationException =
                new InvalidArgumentPersistanceOrchestrationException(
                    message: "Invalid argument persistance orchestration exception, " +
                        "please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidArgumentPersistanceOrchestrationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidArgumentPersistanceOrchestrationException.ThrowIfContainsErrors();
        }
    }
}
