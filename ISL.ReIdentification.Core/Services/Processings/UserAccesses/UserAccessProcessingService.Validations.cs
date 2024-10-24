﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAccesses
{
    public partial class UserAccessProcessingService
    {
        private async ValueTask ValidateOnAddUserAccessAsync(UserAccess userAccess)
        {
            ValidateUserAccessIsNotNull(userAccess);
        }

        private async ValueTask ValidateOnModifyUserAccessAsync(UserAccess userAccess)
        {
            ValidateUserAccessIsNotNull(userAccess);
        }

        private static void ValidateUserAccessIsNotNull(UserAccess userAccess)
        {
            if (userAccess is null)
            {
                throw new NullUserAccessProcessingException("User access is null.");
            }
        }

        private async ValueTask ValidateOnRetrieveUserAccessByIdAsync(Guid userAccessId)
        {
            Validate(
                (Rule: IsInvalid(userAccessId), Parameter: nameof(UserAccess.Id)));
        }

        private async ValueTask ValidateOnRemoveUserAccessByIdAsync(Guid userAccessId)
        {
            Validate(
                (Rule: IsInvalid(userAccessId), Parameter: nameof(UserAccess.Id)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
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
