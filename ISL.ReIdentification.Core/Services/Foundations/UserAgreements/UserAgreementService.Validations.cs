using System;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAgreements
{
    public partial class UserAgreementService
    {
        private void ValidateUserAgreementOnAdd(UserAgreement userAgreement)
        {
            ValidateUserAgreementIsNotNull(userAgreement);

            Validate(
                (Rule: IsInvalid(userAgreement.Id), Parameter: nameof(UserAgreement.Id)),

                // TODO: Add any other required validation rules

                (Rule: IsInvalid(userAgreement.CreatedDate), Parameter: nameof(UserAgreement.CreatedDate)),
                (Rule: IsInvalid(userAgreement.CreatedBy), Parameter: nameof(UserAgreement.CreatedBy)),
                (Rule: IsInvalid(userAgreement.UpdatedDate), Parameter: nameof(UserAgreement.UpdatedDate)),
                (Rule: IsInvalid(userAgreement.UpdatedBy), Parameter: nameof(UserAgreement.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: userAgreement.UpdatedDate,
                    secondDate: userAgreement.CreatedDate,
                    secondDateName: nameof(UserAgreement.CreatedDate)),
                Parameter: nameof(UserAgreement.UpdatedDate)),

                (Rule: IsNotSame(
                    first: userAgreement.UpdatedBy,
                    second: userAgreement.CreatedBy,
                    secondName: nameof(UserAgreement.CreatedBy)),
                Parameter: nameof(UserAgreement.UpdatedBy)),

                (Rule: IsNotRecent(userAgreement.CreatedDate), Parameter: nameof(UserAgreement.CreatedDate)));
        }

        private void ValidateUserAgreementOnModify(UserAgreement userAgreement)
        {
            ValidateUserAgreementIsNotNull(userAgreement);

            Validate(
                (Rule: IsInvalid(userAgreement.Id), Parameter: nameof(UserAgreement.Id)),

                // TODO: Add any other required validation rules

                (Rule: IsInvalid(userAgreement.CreatedDate), Parameter: nameof(UserAgreement.CreatedDate)),
                (Rule: IsInvalid(userAgreement.CreatedBy), Parameter: nameof(UserAgreement.CreatedBy)),
                (Rule: IsInvalid(userAgreement.UpdatedDate), Parameter: nameof(UserAgreement.UpdatedDate)),
                (Rule: IsInvalid(userAgreement.UpdatedBy), Parameter: nameof(UserAgreement.UpdatedBy)),

                (Rule: IsSame(
                    firstDate: userAgreement.UpdatedDate,
                    secondDate: userAgreement.CreatedDate,
                    secondDateName: nameof(UserAgreement.CreatedDate)),
                Parameter: nameof(UserAgreement.UpdatedDate)),

                (Rule: IsNotRecent(userAgreement.UpdatedDate), Parameter: nameof(userAgreement.UpdatedDate)));
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
                Parameter: nameof(UserAgreement.CreatedDate)));
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
           string second,
           string secondName) => new
           {
               Condition = first != second,
               Message = $"Text is not the same as {secondName}"
           };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                this.dateTimeBroker.GetCurrentDateTimeOffset();

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
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