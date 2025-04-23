// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationService : IIdentificationOrchestrationService
    {
        private static void ValidateIdentificationRequestIsNotNull(IdentificationRequest identificationRequest)
        {
            if (identificationRequest is null)
            {
                throw new NullIdentificationRequestException("Identification request is null.");
            }
        }

        private static void ValidateOnExpireRenewImpersonationContextTokensAsync(AccessRequest accessRequest)
        {
            ValidateAccessRequestIsNotNull(accessRequest);

            Validate((Rule: IsInvalid(accessRequest.ImpersonationContext),
                Parameter: nameof(AccessRequest.ImpersonationContext)));
        }

        private static void ValidateOnRemoveDocumentByFileName(string fileName, string container)
        {
            Validate(
                (Rule: IsInvalid(fileName), Parameter: nameof(fileName)),
                (Rule: IsInvalid(container), Parameter: nameof(container)));
        }

        private static void ValidateOnRetrieveDocumentByFileName(Stream output, string fileName, string container)
        {
            Validate(
                (Rule: IsInvalid(fileName), Parameter: nameof(fileName)),
                (Rule: IsInvalid(container), Parameter: nameof(container)),
                (Rule: IsInvalidOutputStream(output), Parameter: nameof(output)));
        }

        private static void ValidateOnAddDocument(Stream input, string fileName, string container)
        {
            Validate(
                (Rule: IsInvalid(fileName), Parameter: nameof(fileName)),
                (Rule: IsInvalid(container), Parameter: nameof(container)),
                (Rule: IsInvalidInputStream(input), Parameter: nameof(input)));
        }

        private static void ValidateAccessRequestIsNotNull(AccessRequest accessRequest)
        {
            if (accessRequest is null)
            {
                throw new NullAccessRequestException("Access request is null.");
            }
        }

        private static dynamic IsInvalid(string value) => new
        {
            Condition = string.IsNullOrWhiteSpace(value),
            Message = "Text is invalid"
        };

        private static dynamic IsInvalid(AccessRequest accessRequest) => new
        {
            Condition = accessRequest is null || accessRequest.ImpersonationContext is null,
            Message = "AccessRequest is invalid"
        };

        private static dynamic IsInvalidOutputStream(Stream outputStream) => new
        {
            Condition = outputStream is null || outputStream.Length > 0,
            Message = "Stream is invalid"
        };

        private static dynamic IsInvalidInputStream(Stream inputStream) => new
        {
            Condition = inputStream is null,
            Message = "Stream is invalid"
        };

        private static dynamic IsInvalid(Object @object) => new
        {
            Condition = @object is null,
            Message = "Object is invalid"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidArgumentIdentificationOrchestrationException =
                new InvalidArgumentIdentificationOrchestrationException(
                    message: "Invalid argument identification orchestration exception, " +
                        "please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidArgumentIdentificationOrchestrationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }

            }

            invalidArgumentIdentificationOrchestrationException.ThrowIfContainsErrors();
        }
    }
}
