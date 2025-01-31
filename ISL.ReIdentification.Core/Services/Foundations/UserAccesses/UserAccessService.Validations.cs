// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAccesses
{
    public partial class UserAccessService
    {
        private async ValueTask ValidateUserAccessOnAddAsync(UserAccess userAccess)
        {
            ValidateUserAccessIsNotNull(userAccess);
            EntraUser auditUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: IsInvalid(userAccess.Id), Parameter: nameof(UserAccess.Id)),
                (Rule: IsInvalid(userAccess.EntraUserId), Parameter: nameof(UserAccess.EntraUserId)),
                (Rule: IsInvalid(userAccess.Email), Parameter: nameof(UserAccess.Email)),
                (Rule: IsInvalid(userAccess.OrgCode), Parameter: nameof(UserAccess.OrgCode)),
                (Rule: IsInvalid(userAccess.CreatedBy), Parameter: nameof(UserAccess.CreatedBy)),
                (Rule: IsInvalid(userAccess.UpdatedBy), Parameter: nameof(UserAccess.UpdatedBy)),
                (Rule: IsInvalid(userAccess.CreatedDate), Parameter: nameof(UserAccess.CreatedDate)),
                (Rule: IsInvalid(userAccess.UpdatedDate), Parameter: nameof(UserAccess.UpdatedDate)),
                (Rule: IsInvalidLength(userAccess.CreatedBy, 255), Parameter: nameof(UserAccess.CreatedBy)),
                (Rule: IsInvalidLength(userAccess.UpdatedBy, 255), Parameter: nameof(UserAccess.UpdatedBy)),
                (Rule: IsInvalidLength(userAccess.Email, 320), Parameter: nameof(UserAccess.Email)),
                (Rule: IsInvalidLength(userAccess.OrgCode, 15), Parameter: nameof(UserAccess.OrgCode)),

                (Rule: IsNotSame(
                    first: userAccess.UpdatedBy,
                    second: userAccess.CreatedBy,
                    secondName: nameof(UserAccess.CreatedBy)),
                Parameter: nameof(UserAccess.UpdatedBy)),

                (Rule: IsNotSame(
                    first: auditUser.EntraUserId.ToString(),
                    second: userAccess.CreatedBy),
                Parameter: nameof(UserAccess.CreatedBy)),

                (Rule: IsNotSame(
                    first: userAccess.CreatedDate,
                    second: userAccess.UpdatedDate,
                    secondName: nameof(UserAccess.CreatedDate)),
                Parameter: nameof(UserAccess.UpdatedBy)));
        }

        private static void ValidateUserAccessOnRetrieveById(Guid userAccessId)
        {
            Validate(
                (Rule: IsInvalid(userAccessId), Parameter: nameof(UserAccess.Id)));
        }

        private async ValueTask ValidateUserAccessOnModifyAsync(UserAccess userAccess)
        {
            ValidateUserAccessIsNotNull(userAccess);

            Validate(
                (Rule: IsInvalid(userAccess.Id), Parameter: nameof(UserAccess.Id)),
                (Rule: IsInvalid(userAccess.EntraUserId), Parameter: nameof(UserAccess.EntraUserId)),
                (Rule: IsInvalid(userAccess.Email), Parameter: nameof(UserAccess.Email)),
                (Rule: IsInvalid(userAccess.OrgCode), Parameter: nameof(UserAccess.OrgCode)),
                (Rule: IsInvalid(userAccess.CreatedBy), Parameter: nameof(UserAccess.CreatedBy)),
                (Rule: IsInvalid(userAccess.UpdatedBy), Parameter: nameof(UserAccess.UpdatedBy)),
                (Rule: IsInvalid(userAccess.CreatedDate), Parameter: nameof(UserAccess.CreatedDate)),
                (Rule: IsInvalid(userAccess.UpdatedDate), Parameter: nameof(UserAccess.UpdatedDate)),
                (Rule: IsInvalidLength(userAccess.CreatedBy, 255), Parameter: nameof(UserAccess.CreatedBy)),
                (Rule: IsInvalidLength(userAccess.UpdatedBy, 255), Parameter: nameof(UserAccess.UpdatedBy)),
                (Rule: IsInvalidLength(userAccess.Email, 320), Parameter: nameof(UserAccess.Email)),
                (Rule: IsInvalidLength(userAccess.OrgCode, 15), Parameter: nameof(UserAccess.OrgCode)),

                (Rule: IsSameAs(
                    createdDate: userAccess.CreatedDate,
                    updatedDate: userAccess.UpdatedDate,
                    createdDateName: nameof(UserAccess.CreatedDate)),

                Parameter: nameof(UserAccess.UpdatedDate)),

                (Rule: await IsNotRecentAsync(userAccess.UpdatedDate), Parameter: nameof(UserAccess.UpdatedDate)));
        }

        private static void ValidateUserAccessOnRemoveById(Guid userAccessId) =>
            Validate((Rule: IsInvalid(userAccessId), Parameter: nameof(UserAccess.Id)));

        private static void ValidateOnRetrieveAllOrganisationUserHasAccessTo(Guid userAccessId) =>
            Validate((Rule: IsInvalid(userAccessId), Parameter: nameof(UserAccess.Id)));

        private static void ValidateStorageUserAccess(UserAccess maybeUserAccess, Guid maybeId)
        {
            if (maybeUserAccess is null)
            {
                throw new NotFoundUserAccessException($"User access not found with Id: {maybeId}");
            }
        }

        private static void ValidateAgainstStorageUserAccessOnModify(
            UserAccess userAccess,
            UserAccess maybeUserAccess)
        {
            Validate(
                (Rule: IsNotSame(
                    userAccess.CreatedDate,
                    maybeUserAccess.CreatedDate,
                    nameof(maybeUserAccess.CreatedDate)),

                Parameter: nameof(UserAccess.CreatedDate)),

                (Rule: IsSameAs(
                    userAccess.UpdatedDate,
                    maybeUserAccess.UpdatedDate,
                    nameof(maybeUserAccess.UpdatedDate)),

                Parameter: nameof(UserAccess.UpdatedDate)));
        }

        private static void ValidateUserAccessIsNotNull(UserAccess userAccess)
        {
            if (userAccess is null)
            {
                throw new NullUserAccessException("User access is null.");
            }
        }

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

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is invalid"
        };

        private static dynamic IsInvalidLength(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static dynamic IsNotSame(
            DateTimeOffset first,
            DateTimeOffset second) => new
            {
                Condition = first != second,
                Message = $"Expected value to be {first} but found {second}."
            };

        private static dynamic IsNotSame(
            DateTimeOffset first,
            DateTimeOffset second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Date is not the same as {secondName}"
            };

        private static dynamic IsNotSame(
            string first,
            string second) => new
            {
                Condition = first != second,
                Message = $"Expected value to be {first} but found {second}."
            };

        private static dynamic IsNotSame(
            string first,
            string second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Text is not the same as {secondName}"
            };


        private static dynamic IsSameAs(
            DateTimeOffset createdDate,
            DateTimeOffset updatedDate,
            string createdDateName) => new
            {
                Condition = createdDate == updatedDate,
                Message = $"Date is the same as {createdDateName}"
            };

        private async ValueTask<dynamic> IsNotRecentAsync(DateTimeOffset date)
        {
            var (isNotRecent, startDate, endDate) = await IsDateNotRecentAsync(date);

            return new
            {
                Condition = isNotRecent,
                Message = $"Date is not recent. Expected a value between {startDate} and {endDate} but found {date}"
            };
        }

        private async ValueTask<(bool IsNotRecent, DateTimeOffset StartDate, DateTimeOffset EndDate)>
            IsDateNotRecentAsync(DateTimeOffset date)
        {
            int pastThreshold = 90;
            int futureThreshold = 0;
            DateTimeOffset currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            if (currentDateTime == default)
            {
                return (false, default, default);
            }

            DateTimeOffset startDate = currentDateTime.AddSeconds(-pastThreshold);
            DateTimeOffset endDate = currentDateTime.AddSeconds(futureThreshold);
            bool isNotRecent = date < startDate || date > endDate;

            return (isNotRecent, startDate, endDate);
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserAccessException =
                new InvalidUserAccessException(
                    message: "Invalid user access. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserAccessException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidUserAccessException.ThrowIfContainsErrors();
        }
    }
}
