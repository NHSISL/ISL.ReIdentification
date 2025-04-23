// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ISL.ReIdentification.Core.Models.Foundations.Documents;
using ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions;

namespace ISL.ReIdentification.Core.Services.Foundations.Documents
{
    public partial class DocumentService : IDocumentService
    {
        private static void ValidateOnAddDocument(Stream input, string fileName, string container)
        {
            Validate(
                (Rule: IsInvalid(fileName), Parameter: nameof(fileName)),
                (Rule: IsInvalid(container), Parameter: nameof(container)),
                (Rule: IsInvalidInputStream(input), Parameter: nameof(input)));
        }

        private static void ValidateOnRetrieveDocumentByFileName(Stream output, string fileName, string container)
        {
            Validate(
                (Rule: IsInvalid(fileName), Parameter: nameof(fileName)),
                (Rule: IsInvalid(container), Parameter: nameof(container)),
                (Rule: IsInvalidOutputStream(output), Parameter: nameof(output)));
        }

        private static void ValidateOnRemoveDocumentByFileName(string fileName, string container)
        {
            Validate(
                (Rule: IsInvalid(fileName), Parameter: nameof(fileName)),
                (Rule: IsInvalid(container), Parameter: nameof(container)));
        }

        private static void ValidateStorageArgumentsOnRetrieveAccessPolicies(string container)
        {
            Validate((Rule: IsInvalid(container), Parameter: "Container"));
        }

        private static void ValidateStorageArgumentsOnRetrieveAccessPolicyByName(string container, string policyName)
        {
            Validate(
                (Rule: IsInvalid(container), Parameter: "Container"),
                (Rule: IsInvalid(policyName), Parameter: "PolicyName"));
        }

        private static void ValidateStorageArgumentsOnRemoveAccessPolicies(string container) =>
            Validate((Rule: IsInvalid(container), Parameter: "Container"));

        private static void ValidateStorageArgumentsOnRemoveAccessPolicyByName(string container, string policyName)
        {
            Validate(
                (Rule: IsInvalid(container), Parameter: "Container"),
                (Rule: IsInvalid(policyName), Parameter: "PolicyName"));
        }

        private static void ValidateOnAddContainer(string container) =>
            Validate((Rule: IsInvalid(container), Parameter: nameof(container)));

        private static void ValidateOnListFilesInContainer(string container) =>
            Validate((Rule: IsInvalid(container), Parameter: "Container"));

        private static void ValidateOnAddFolder(string folderName, string container)
        {
            Validate(
                (Rule: IsInvalid(folderName), Parameter: nameof(folderName)),
                (Rule: IsInvalid(container), Parameter: nameof(container)));
        }

        private static void ValidateOnGetDownloadLink(
            string folderName,
            string container,
            DateTimeOffset dateTimeOffset)
        {
            Validate(
                (Rule: IsInvalid(folderName), Parameter: nameof(folderName)),
                (Rule: IsInvalid(container), Parameter: nameof(container)),
                (Rule: IsInvalid(dateTimeOffset), Parameter: nameof(dateTimeOffset)));
        }

        private static void ValidateOnCreateDirectorySasToken(
            string container,
            string path,
            string accessPolicyIdentifier,
            DateTimeOffset expiresOn)
        {
            Validate(
                (Rule: IsInvalid(container), Parameter: nameof(container)),
                (Rule: IsInvalid(path), Parameter: nameof(path)),
                (Rule: IsInvalid(accessPolicyIdentifier), Parameter: nameof(accessPolicyIdentifier)),
                (Rule: IsInvalid(expiresOn), Parameter: nameof(expiresOn)));
        }

        private static void ValidateOnCreateAndAssignAccessPolicies(string container, List<AccessPolicy> accessPolicies)
        {
            Validate(
                (Rule: IsInvalid(container), Parameter: nameof(container)),
                (Rule: IsInvalid(accessPolicies), Parameter: nameof(accessPolicies)));

            foreach (var accessPolicy in accessPolicies)
            {
                Validate(
                    (Rule: IsInvalid(accessPolicy.PolicyName),
                    Parameter: $"{nameof(AccessPolicy)}.{nameof(AccessPolicy.PolicyName)}"),

                    (Rule: IsInvalid(accessPolicy.Permissions),
                    Parameter: $"{nameof(AccessPolicy)}.{nameof(AccessPolicy.Permissions)}"));
            }

            ValidateAccessPolicyPermissions(accessPolicies);
        }

        private static void ValidateAccessPolicyExists(string policyName, List<string> policyNames)
        {
            if (!(policyNames.Any(policy => policy == policyName)))
            {
                throw new AccessPolicyNotFoundDocumentException(
                    message: "Access policy with the provided name was not found on this container.");
            }
        }

        private static void ValidateAccessPolicyPermissions(List<AccessPolicy> accessPolicies)
        {
            foreach (var accessPolicy in accessPolicies)
            {
                ValidatePermissions(accessPolicy.Permissions);
            }
        }

        private static void ValidatePermissions(List<string> permissions)
        {
            foreach (var permission in permissions)
            {
                if (permission.ToLower() != "read" &&
                    permission.ToLower() != "write" &&
                    permission.ToLower() != "delete" &&
                    permission.ToLower() != "create" &&
                    permission.ToLower() != "add" &&
                    permission.ToLower() != "list")
                {
                    throw new InvalidPermissionDocumentException(
                        message: "Invalid permission. Read, write, delete, create, add and list " +
                        "permissions are supported at this time.");
                }
            }
        }

        private static dynamic IsInvalid(DateTimeOffset dateTimeOffset) => new
        {
            Condition = dateTimeOffset == default || dateTimeOffset <= DateTimeOffset.UtcNow,
            Message = "Date is invalid"
        };

        private static dynamic IsInvalid(string value) => new
        {
            Condition = string.IsNullOrWhiteSpace(value),
            Message = "Text is invalid"
        };

        private static dynamic IsInvalid(List<string> textList) => new
        {
            Condition = textList is null || textList.Count == 0 || textList.Any(string.IsNullOrWhiteSpace),
            Message = "List is invalid"
        };

        private static dynamic IsInvalid(List<AccessPolicy> accessPolicies) => new
        {
            Condition = accessPolicies is null || accessPolicies.Count == 0,
            Message = "List is invalid"
        };

        private static dynamic IsInvalidInputStream(Stream inputStream) => new
        {
            Condition = inputStream is null,
            Message = "Stream is invalid"
        };

        private static dynamic IsInvalidOutputStream(Stream outputStream) => new
        {
            Condition = outputStream is null || outputStream.Length > 0,
            Message = "Stream is invalid"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidDocumentException = new InvalidDocumentException(
                message: "Invalid document. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDocumentException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDocumentException.ThrowIfContainsErrors();
        }
    }
}