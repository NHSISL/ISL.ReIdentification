// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;

namespace ISL.ReIdentification.Core.Services.Coordinations.Identifications
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

        private static void ValidateOnPersistImpersonationContext(AccessRequest accessRequest)
        {
            ValidateAccessRequestIsNotNull(accessRequest);
            Validate((Rule: IsInvalid(accessRequest.ImpersonationContext),
                Parameter: nameof(AccessRequest.ImpersonationContext)));
        }

        private static void ValidateOnProcessImpersonationContextRequestAsync(
            string container,
            string filepath,
            ProjectStorageConfiguration projectStorageConfiguration)
        {
            Validate(
                (Rule: IsInvalid(projectStorageConfiguration), Parameter: nameof(ProjectStorageConfiguration)));

            Validate(
                (Rule: IsInvalid(container), Parameter: nameof(container)),
                (Rule: IsInvalid(filepath), Parameter: nameof(filepath)),
                (Rule: IsInvalid(projectStorageConfiguration.Container),
                Parameter: $"{nameof(ProjectStorageConfiguration)}." +
                    $"{nameof(ProjectStorageConfiguration.Container)}"),

                (Rule: IsInvalid(projectStorageConfiguration.LandingFolder),
                Parameter: $"{nameof(ProjectStorageConfiguration)}." +
                    $"{nameof(ProjectStorageConfiguration.LandingFolder)}"),

                (Rule: IsInvalid(projectStorageConfiguration.PickupFolder),
                Parameter: $"{nameof(ProjectStorageConfiguration)}." +
                    $"{nameof(ProjectStorageConfiguration.PickupFolder)}"),

                (Rule: IsInvalid(projectStorageConfiguration.ErrorFolder),
                Parameter: $"{nameof(ProjectStorageConfiguration)}." +
                    $"{nameof(ProjectStorageConfiguration.ErrorFolder)}"));
        }


        private void ValidateCsvData(List<IdentificationItem> mappedItems)
        {
            List<(dynamic Rule, string Parameter)> rules = new List<(dynamic Rule, string Parameter)>();

            foreach (IdentificationItem item in mappedItems)
            {
                rules.Add(
                    (
                        Rule: IsInvalidRow(item.Identifier, item.RowNumber),
                        Parameter: nameof(CsvIdentificationItem.Identifier))
                    );
            }

            Validate(rules.ToArray());
        }

        private static void ValidateAccessRequestIsNotNull(AccessRequest accessRequest)
        {
            if (accessRequest is null)
            {
                throw new NullAccessRequestException("Access request is null.");
            }
        }

        public static void ValidateOnProcessCsvIdentificationRequest(Guid csvIdentificationRequestId, string reason) =>
            Validate(
                (Rule: IsInvalid(csvIdentificationRequestId), Parameter: nameof(CsvIdentificationRequest.Id)),
                (Rule: IsInvalid(reason), Parameter: nameof(CsvIdentificationRequest.Reason)));

        private static void ValidateOnExpireRenewImpersonationContextTokens(Guid impersonationContextId) =>
            Validate((Rule: IsInvalid(impersonationContextId), Parameter: nameof(ImpersonationContext.Id)));

        private static void ValidateOnImpersonationContextApproval(Guid impersonationContextId) =>
            Validate((Rule: IsInvalid(impersonationContextId), Parameter: nameof(ImpersonationContext.Id)));

        private static void ValidateUserAccessOnImpersonationContextApproval(
            string currentUserEntraId,
            string responsiblePersonEntraId)
        {
            if (currentUserEntraId != responsiblePersonEntraId)
            {
                throw new InvalidAccessIdentificationCoordinationException(
                    message: "Invalid access. Please contact support.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        private static dynamic IsInvalid(string value) => new
        {
            Condition = string.IsNullOrWhiteSpace(value),
            Message = "Text is invalid"
        };

        private static dynamic IsInvalidRow(string value, string index) => new
        {
            Condition = string.IsNullOrWhiteSpace(value),
            Message = $"Text is invalid at row {index}"
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
