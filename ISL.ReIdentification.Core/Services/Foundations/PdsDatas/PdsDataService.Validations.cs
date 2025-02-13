// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;

namespace ISL.ReIdentification.Core.Services.Foundations.PdsDatas
{
    public partial class PdsDataService
    {
        private async ValueTask ValidatePdsDataOnAddAsync(PdsData pdsData)
        {
            ValidatePdsDataIsNotNull(pdsData);
            EntraUser currentUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: IsInvalid(pdsData.Id), Parameter: nameof(PdsData.Id)),
                (Rule: IsInvalid(pdsData.PseudoNhsNumber), Parameter: nameof(PdsData.PseudoNhsNumber)),
                (Rule: IsInvalid(pdsData.OrgCode), Parameter: nameof(PdsData.OrgCode)),
                (Rule: IsInvalid(pdsData.OrganisationName), Parameter: nameof(PdsData.OrganisationName)),
                (Rule: IsInvalid(pdsData.CreatedDate), Parameter: nameof(PdsData.CreatedDate)),
                (Rule: IsInvalid(pdsData.CreatedBy), Parameter: nameof(PdsData.CreatedBy)),
                (Rule: IsInvalid(pdsData.UpdatedDate), Parameter: nameof(PdsData.UpdatedDate)),
                (Rule: IsInvalid(pdsData.UpdatedBy), Parameter: nameof(PdsData.UpdatedBy)),
                (Rule: IsInvalidLength(pdsData.PseudoNhsNumber, 10), Parameter: nameof(PdsData.PseudoNhsNumber)),
                (Rule: IsInvalidLength(pdsData.OrgCode, 50), Parameter: nameof(PdsData.OrgCode)),
                (Rule: IsInvalidLength(pdsData.OrganisationName, 255), Parameter: nameof(PdsData.OrganisationName)),
                (Rule: IsInvalidLength(pdsData.CreatedBy, 255), Parameter: nameof(PdsData.CreatedBy)),
                (Rule: IsInvalidLength(pdsData.UpdatedBy, 255), Parameter: nameof(PdsData.UpdatedBy)),

                (Rule: IsNotSame(
                    first: currentUser.EntraUserId,
                    second: pdsData.CreatedBy),
                Parameter: nameof(PdsData.CreatedBy)),

                (Rule: IsNotSame(
                    first: pdsData.UpdatedBy,
                    second: pdsData.CreatedBy,
                    secondName: nameof(PdsData.CreatedBy)),
                Parameter: nameof(PdsData.UpdatedBy)),

                (Rule: IsNotSame(
                    first: pdsData.UpdatedDate,
                    second: pdsData.CreatedDate,
                    secondName: nameof(PdsData.CreatedDate)),
                Parameter: nameof(PdsData.UpdatedDate)),

                (Rule: await IsNotRecentAsync(pdsData.CreatedDate), Parameter: nameof(PdsData.CreatedDate))
            );
        }

        private async ValueTask ValidatePdsDataOnModifyAsync(PdsData pdsData)
        {
            ValidatePdsDataIsNotNull(pdsData);
            EntraUser currentUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: IsInvalid(pdsData.Id), Parameter: nameof(PdsData.Id)),
                (Rule: IsInvalid(pdsData.PseudoNhsNumber), Parameter: nameof(PdsData.PseudoNhsNumber)),
                (Rule: IsInvalid(pdsData.OrgCode), Parameter: nameof(PdsData.OrgCode)),
                (Rule: IsInvalid(pdsData.OrganisationName), Parameter: nameof(PdsData.OrganisationName)),
                (Rule: IsInvalid(pdsData.CreatedDate), Parameter: nameof(PdsData.CreatedDate)),
                (Rule: IsInvalid(pdsData.CreatedBy), Parameter: nameof(PdsData.CreatedBy)),
                (Rule: IsInvalid(pdsData.UpdatedDate), Parameter: nameof(PdsData.UpdatedDate)),
                (Rule: IsInvalid(pdsData.UpdatedBy), Parameter: nameof(PdsData.UpdatedBy)),
                (Rule: IsInvalidLength(pdsData.PseudoNhsNumber, 10), Parameter: nameof(PdsData.PseudoNhsNumber)),
                (Rule: IsInvalidLength(pdsData.OrgCode, 50), Parameter: nameof(PdsData.OrgCode)),
                (Rule: IsInvalidLength(pdsData.OrganisationName, 255), Parameter: nameof(PdsData.OrganisationName)),
                (Rule: IsInvalidLength(pdsData.CreatedBy, 255), Parameter: nameof(PdsData.CreatedBy)),
                (Rule: IsInvalidLength(pdsData.UpdatedBy, 255), Parameter: nameof(PdsData.UpdatedBy)),

                (Rule: IsNotSame(
                    first: currentUser.EntraUserId,
                    second: pdsData.UpdatedBy),
                Parameter: nameof(PdsData.UpdatedBy)),

                (Rule: IsSameAs(
                    firstDate: pdsData.CreatedDate,
                    secondDate: pdsData.UpdatedDate,
                    secondDateName: nameof(PdsData.CreatedDate)),
                Parameter: nameof(PdsData.UpdatedDate)),

                (Rule: await IsNotRecentAsync(pdsData.UpdatedDate), Parameter: nameof(PdsData.UpdatedDate))
            );
        }

        private static void ValidateOnOrganisationsHaveAccessToThisPatient(
            string pseudoNhsNumber,
            List<string> organisationCodes)
        {
            Validate(
                (Rule: IsInvalid(pseudoNhsNumber), Parameter: nameof(pseudoNhsNumber)),
                (Rule: IsInvalid(organisationCodes), Parameter: nameof(organisationCodes)));
        }

        public static void ValidatePdsDataId(Guid pdsDataId) =>
            Validate((Rule: IsInvalid(pdsDataId), Parameter: nameof(PdsData.Id)));

        private static void ValidateStoragePdsData(PdsData maybePdsData, Guid pdsDataId)
        {
            if (maybePdsData is null)
            {
                throw new NotFoundPdsDataException(message: $"PdsData not found with Id: {pdsDataId}");
            }
        }

        private static void ValidatePdsDataIsNotNull(PdsData pdsData)
        {
            if (pdsData is null)
            {
                throw new NullPdsDataException(message: "PdsData is null.");
            }
        }

        private static void ValidateAgainstStoragePdsDataOnModify(PdsData inputPdsData, PdsData storagePdsData)
        { }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        private static dynamic IsInvalid(List<string> organisationCodes) => new
        {
            Condition = organisationCodes is null || organisationCodes.Count == 0,
            Message = "Items is invalid"
        };

        private static dynamic IsInvalid(long id) => new
        {
            Condition = id == default,
            Message = "Id is invalid"
        };

        private static dynamic IsInvalid(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Text is invalid"
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
            Message = "Date is invalid"
        };

        private static dynamic IsSameAs(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
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

        private static dynamic IsNotSame(
            DateTimeOffset first,
            DateTimeOffset second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Date is not the same as {secondName}"
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
            var invalidPdsDataException =
                new InvalidPdsDataException(
                    message: "Invalid pdsData. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidPdsDataException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidPdsDataException.ThrowIfContainsErrors();
        }
    }
}