// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAccesses
{
    public partial class UserAccessProcessingService
    {
        private static void ValidateOnAddUserAccess(UserAccess userAccess) =>
            ValidateUserAccessIsNotNull(userAccess);

        private static void ValidateOnModifyUserAccess(UserAccess userAccess) =>
            ValidateUserAccessIsNotNull(userAccess);

        private static void ValidateOnBulkAddRemoveUserAccessAsync(BulkUserAccess bulkUserAccess)
        {
            ValidateBulkUserAccessIsNotNull(bulkUserAccess);

            Validate(
                (Rule: IsInvalid(bulkUserAccess.EntraUserId), Parameter: nameof(BulkUserAccess.EntraUserId)),
                (Rule: IsInvalid(bulkUserAccess.Email), Parameter: nameof(BulkUserAccess.Email)),
                (Rule: IsInvalid(bulkUserAccess.OrgCodes), Parameter: nameof(BulkUserAccess.OrgCodes)));
        }

        private static void ValidateUserAccessIsNotNull(UserAccess userAccess)
        {
            if (userAccess is null)
            {
                throw new NullUserAccessProcessingException("User access is null.");
            }
        }

        private static void ValidateBulkUserAccessIsNotNull(BulkUserAccess bulkUserAccess)
        {
            if (bulkUserAccess is null)
            {
                throw new NullUserAccessProcessingException("Bulk user access is null.");
            }
        }

        private static void ValidateOnRetrieveUserAccessById(Guid userAccessId) =>
            Validate((Rule: IsInvalid(userAccessId), Parameter: nameof(UserAccess.Id)));

        private static void ValidateOnRemoveUserAccessById(Guid userAccessId) =>
            Validate((Rule: IsInvalid(userAccessId), Parameter: nameof(UserAccess.Id)));

        private static void ValidateOnRetrieveAllActiveOrganisationsUserHasAccessTo(Guid userAccessId) =>
            Validate((Rule: IsInvalid(userAccessId), Parameter: nameof(UserAccess.Id)));

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        private static dynamic IsInvalid(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Text is invalid"
        };

        private static dynamic IsInvalid(List<string> list) => new
        {
            Condition = list is null || list.Count == 0,
            Message = "List is invalid"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserAccessProcessingException =
                new InvalidUserAccessProcessingException(
                    message: "Invalid user access. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserAccessProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidUserAccessProcessingException.ThrowIfContainsErrors();
        }
    }
}
