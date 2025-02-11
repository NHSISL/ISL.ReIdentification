// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAgreements
{
    public partial class UserAgreementService
    {
        private async ValueTask ValidateUserAgreementOnAddAsync(UserAgreement userAgreement)
        {
            ValidateUserAgreementIsNotNull(userAgreement);
            EntraUser currentUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: IsInvalid(userAgreement.Id), Parameter: nameof(UserAgreement.Id)),
                (Rule: IsInvalid(userAgreement.EntraUserId), Parameter: nameof(UserAgreement.EntraUserId)),
                (Rule: IsInvalid(userAgreement.AgreementType), Parameter: nameof(UserAgreement.AgreementType)),
                (Rule: IsInvalid(userAgreement.AgreementVersion), Parameter: nameof(UserAgreement.AgreementVersion)),
                (Rule: IsInvalid(userAgreement.AgreementDate), Parameter: nameof(UserAgreement.AgreementDate)),
                (Rule: IsInvalid(userAgreement.CreatedDate), Parameter: nameof(UserAgreement.CreatedDate)),
                (Rule: IsInvalid(userAgreement.CreatedBy), Parameter: nameof(UserAgreement.CreatedBy)),
                (Rule: IsInvalid(userAgreement.UpdatedDate), Parameter: nameof(UserAgreement.UpdatedDate)),
                (Rule: IsInvalid(userAgreement.UpdatedBy), Parameter: nameof(UserAgreement.UpdatedBy)),
                (Rule: IsInvalidLength(userAgreement.EntraUserId, 255), Parameter: nameof(UserAgreement.EntraUserId)),
                (Rule: IsInvalidLength(userAgreement.AgreementType, 255), Parameter: nameof(UserAgreement.AgreementType)),
                (Rule: IsInvalidLength(userAgreement.AgreementVersion, 50), Parameter: nameof(UserAgreement.AgreementVersion)),
                (Rule: IsInvalidLength(userAgreement.CreatedBy, 255), Parameter: nameof(UserAgreement.CreatedBy)),
                (Rule: IsInvalidLength(userAgreement.UpdatedBy, 255), Parameter: nameof(UserAgreement.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: userAgreement.UpdatedDate,
                    secondDate: userAgreement.CreatedDate,
                    secondDateName: nameof(UserAgreement.CreatedDate)),
                Parameter: nameof(UserAgreement.UpdatedDate)),

                (Rule: IsNotSame(
                    first: currentUser.EntraUserId,
                    second: userAgreement.CreatedBy),
                Parameter: nameof(UserAccess.CreatedBy)),

                (Rule: IsNotSame(
                    first: userAgreement.UpdatedBy,
                    second: userAgreement.CreatedBy,
                    secondName: nameof(UserAgreement.CreatedBy)),
                Parameter: nameof(UserAgreement.UpdatedBy)),

                (Rule: await IsNotRecentAsync(userAgreement.CreatedDate), Parameter: nameof(UserAgreement.CreatedDate)));
        }

        private async ValueTask ValidateUserAgreementOnModifyAsync(UserAgreement userAgreement)
        {
            ValidateUserAgreementIsNotNull(userAgreement);
            EntraUser currentUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: IsInvalid(userAgreement.Id), Parameter: nameof(UserAgreement.Id)),
                (Rule: IsInvalid(userAgreement.EntraUserId), Parameter: nameof(UserAgreement.EntraUserId)),
                (Rule: IsInvalid(userAgreement.AgreementType), Parameter: nameof(UserAgreement.AgreementType)),
                (Rule: IsInvalid(userAgreement.AgreementVersion), Parameter: nameof(UserAgreement.AgreementVersion)),
                (Rule: IsInvalid(userAgreement.AgreementDate), Parameter: nameof(UserAgreement.AgreementDate)),
                (Rule: IsInvalid(userAgreement.CreatedDate), Parameter: nameof(UserAgreement.CreatedDate)),
                (Rule: IsInvalid(userAgreement.CreatedBy), Parameter: nameof(UserAgreement.CreatedBy)),
                (Rule: IsInvalid(userAgreement.UpdatedDate), Parameter: nameof(UserAgreement.UpdatedDate)),
                (Rule: IsInvalid(userAgreement.UpdatedBy), Parameter: nameof(UserAgreement.UpdatedBy)),
                (Rule: IsInvalidLength(userAgreement.EntraUserId, 255), Parameter: nameof(UserAgreement.EntraUserId)),
                (Rule: IsInvalidLength(userAgreement.AgreementType, 255), Parameter: nameof(UserAgreement.AgreementType)),
                (Rule: IsInvalidLength(userAgreement.AgreementVersion, 50), Parameter: nameof(UserAgreement.AgreementVersion)),
                (Rule: IsInvalidLength(userAgreement.CreatedBy, 255), Parameter: nameof(UserAgreement.CreatedBy)),
                (Rule: IsInvalidLength(userAgreement.UpdatedBy, 255), Parameter: nameof(UserAgreement.UpdatedBy)),

                (Rule: IsNotSame(
                    first: currentUser.EntraUserId,
                    second: userAgreement.UpdatedBy),
                Parameter: nameof(UserAccess.UpdatedBy)),

                (Rule: IsSame(
                    firstDate: userAgreement.UpdatedDate,
                    secondDate: userAgreement.CreatedDate,
                    secondDateName: nameof(UserAgreement.CreatedDate)),
                Parameter: nameof(UserAgreement.UpdatedDate)),

                (Rule: await IsNotRecentAsync(userAgreement.UpdatedDate), Parameter: nameof(userAgreement.UpdatedDate)));
        }

        public void ValidateUserAgreementId(Guid userAgreementId) =>
            Validate((Rule: IsInvalid(userAgreementId), Parameter: nameof(UserAgreement.Id)));

        private static void ValidateStorageUserAgreement(UserAgreement maybeUserAgreement, Guid userAgreementId)
        {
            if (maybeUserAgreement is null)
            {
                throw new NotFoundUserAgreementException(userAgreementId);
            }
        }

        private static void ValidateUserAgreementIsNotNull(UserAgreement userAgreement)
        {
            if (userAgreement is null)
            {
                throw new NullUserAgreementException(message: "UserAgreement is null.");
            }
        }

        private static void ValidateAgainstStorageUserAgreementOnModify(UserAgreement inputUserAgreement, UserAgreement storageUserAgreement)
        {
            Validate(
                (Rule: IsNotSame(
                    firstDate: inputUserAgreement.CreatedDate,
                    secondDate: storageUserAgreement.CreatedDate,
                    secondDateName: nameof(UserAgreement.CreatedDate)),
                Parameter: nameof(UserAgreement.CreatedDate)),

                (Rule: IsNotSame(
                    first: inputUserAgreement.CreatedBy,
                    second: storageUserAgreement.CreatedBy,
                    secondName: nameof(UserAgreement.CreatedBy)),
                Parameter: nameof(UserAgreement.CreatedBy)),

                (Rule: IsSame(
                    firstDate: inputUserAgreement.UpdatedDate,
                    secondDate: storageUserAgreement.UpdatedDate,
                    secondDateName: nameof(UserAgreement.UpdatedDate)),
                Parameter: nameof(UserAgreement.UpdatedDate)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalidLength(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            Guid firstId,
            Guid secondId,
            string secondIdName) => new
            {
                Condition = firstId != secondId,
                Message = $"Id is not the same as {secondIdName}"
            };

        private static dynamic IsNotSame(
            string first,
            string second) => new
            {
                Condition = first != second,
                Message = $"Expected value to be '{first}' but found '{second}'."
            };

        private static dynamic IsNotSame(
           string first,
           string second,
           string secondName) => new
           {
               Condition = first != second,
               Message = $"Text is not the same as {secondName}"
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
            var invalidUserAgreementException =
                new InvalidUserAgreementException(
                    message: "Invalid userAgreement. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserAgreementException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidUserAgreementException.ThrowIfContainsErrors();
        }
    }
}