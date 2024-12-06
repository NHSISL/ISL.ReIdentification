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
                (Rule: IsInvalid(userAgreement.UpdatedBy), Parameter: nameof(UserAgreement.UpdatedBy)));
        }

        private static void ValidateUserAgreementIsNotNull(UserAgreement userAgreement)
        {
            if (userAgreement is null)
            {
                throw new NullUserAgreementException(message: "UserAgreement is null.");
            }
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