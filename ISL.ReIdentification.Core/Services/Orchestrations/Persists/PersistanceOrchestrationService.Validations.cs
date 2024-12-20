// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationService
    {
        private static void ValidateOnRetrieveImpersonationContextByIdAsync(
            Guid impersonationContextId)
        {
            Validate(
                (Rule: IsInvalid(impersonationContextId), Parameter: nameof(impersonationContextId)));
        }

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

        private static void ValidateOnPersistCsvIdentificationRequestAsync(
             CsvIdentificationRequest csvIdentificationRequest)
        {
            Validate(
                (Rule: IsInvalid(csvIdentificationRequest), Parameter: nameof(csvIdentificationRequest)));
        }

        private static void ValidateOnPersistImpersonationContextAsync(
             AccessRequest accessRequest)
        {
            ValidateAccessRequestIsNotNull(accessRequest);

            Validate(
                (Rule: IsInvalid(accessRequest.ImpersonationContext),
                    Parameter: nameof(accessRequest.ImpersonationContext)));
        }

        private static void ValidateCsvReIdentificationConfigurationIsNotNull(
            CsvReIdentificationConfigurations csvReIdentificationConfigurations)
        {
            if (csvReIdentificationConfigurations is null)
            {
                throw new NullCsvReIdentificationConfigurationException("Csv reidentification configuration is null.");
            }
        }

        private static void ValidateOnPurgeCsvIdentificationRecordsThatExpiredAsync(
             CsvReIdentificationConfigurations csvReIdentificationConfigurations)
        {
            ValidateCsvReIdentificationConfigurationIsNotNull(csvReIdentificationConfigurations);

            Validate(
                (Rule: IsInvalid(csvReIdentificationConfigurations.ExpireAfterMinutes),
                    Parameter: $"{nameof(CsvReIdentificationConfigurations)}.{nameof(CsvReIdentificationConfigurations.ExpireAfterMinutes)}"));
        }

        private static void ValidateOnExpireRenewImpersonationContextTokensAsync(AccessRequest accessRequest)
        {
            Validate(
               (Rule: IsInvalid(accessRequest), Parameter: nameof(accessRequest)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        private static dynamic IsInvalid(AccessRequest accessRequest) => new
        {
            Condition = accessRequest is null || accessRequest.ImpersonationContext is null,
            Message = "AccessRequest is invalid"
        };

        private static dynamic IsInvalid(CsvIdentificationRequest csvIdentificationRequest) => new
        {
            Condition = csvIdentificationRequest == null,
            Message = "AccessRequest is invalid"
        };

        private static dynamic IsInvalid(ImpersonationContext impersonationContext) => new
        {
            Condition = impersonationContext == null,
            Message = "AccessRequest is invalid"
        };

        private static dynamic IsInvalid(int value) => new
        {
            Condition = value <= 5,
            Message = "Value is invalid"
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
