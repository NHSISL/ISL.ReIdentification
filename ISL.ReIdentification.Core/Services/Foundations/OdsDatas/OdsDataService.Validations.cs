// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;

namespace ISL.ReIdentification.Core.Services.Foundations.OdsDatas
{
    public partial class OdsDataService
    {
        private async ValueTask ValidateOdsDataOnAddAsync(OdsData odsData)
        {
            ValidateOdsDataIsNotNull(odsData);
            EntraUser currentUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: IsInvalid(odsData.Id), Parameter: nameof(OdsData.Id)),
                (Rule: IsInvalid(odsData.OrganisationCode), Parameter: nameof(OdsData.OrganisationCode)),
                (Rule: IsInvalid(odsData.OrganisationName), Parameter: nameof(OdsData.OrganisationName)),
                (Rule: IsInvalid(odsData.CreatedDate), Parameter: nameof(OdsData.CreatedDate)),
                (Rule: IsInvalid(odsData.CreatedBy), Parameter: nameof(OdsData.CreatedBy)),
                (Rule: IsInvalid(odsData.UpdatedDate), Parameter: nameof(OdsData.UpdatedDate)),
                (Rule: IsInvalid(odsData.UpdatedBy), Parameter: nameof(OdsData.UpdatedBy)),

                (Rule: IsInvalidLength(odsData.OrganisationCode, 15), Parameter: nameof(OdsData.OrganisationCode)),
                (Rule: IsInvalidLength(odsData.OrganisationName, 220), Parameter: nameof(OdsData.OrganisationName)),
                (Rule: IsInvalidLength(odsData.CreatedBy, 255), Parameter: nameof(OdsData.CreatedBy)),
                (Rule: IsInvalidLength(odsData.UpdatedBy, 255), Parameter: nameof(OdsData.UpdatedBy)),

                (Rule: IsNotSame(
                    first: currentUser.EntraUserId,
                    second: odsData.CreatedBy),
                Parameter: nameof(OdsData.CreatedBy)),

                (Rule: IsNotSame(
                    first: odsData.UpdatedBy,
                    second: odsData.CreatedBy,
                    secondName: nameof(OdsData.CreatedBy)),
                Parameter: nameof(OdsData.UpdatedBy)),

                (Rule: IsNotSame(
                    first: odsData.UpdatedDate,
                    second: odsData.CreatedDate,
                    secondName: nameof(OdsData.CreatedDate)),
                Parameter: nameof(OdsData.UpdatedDate)),

                (Rule: await IsNotRecentAsync(odsData.CreatedDate), Parameter: nameof(OdsData.CreatedDate))
            );
        }

        private async ValueTask ValidateOdsDataOnModifyAsync(OdsData odsData)
        {
            ValidateOdsDataIsNotNull(odsData);
            EntraUser currentUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: IsInvalid(odsData.Id), Parameter: nameof(OdsData.Id)),
                (Rule: IsInvalid(odsData.OrganisationCode), Parameter: nameof(OdsData.OrganisationCode)),
                (Rule: IsInvalid(odsData.OrganisationName), Parameter: nameof(OdsData.OrganisationName)),
                (Rule: IsInvalid(odsData.CreatedDate), Parameter: nameof(OdsData.CreatedDate)),
                (Rule: IsInvalid(odsData.CreatedBy), Parameter: nameof(OdsData.CreatedBy)),
                (Rule: IsInvalid(odsData.UpdatedDate), Parameter: nameof(OdsData.UpdatedDate)),
                (Rule: IsInvalid(odsData.UpdatedBy), Parameter: nameof(OdsData.UpdatedBy)),

                (Rule: IsInvalidLength(odsData.OrganisationCode, 15), Parameter: nameof(OdsData.OrganisationCode)),
                (Rule: IsInvalidLength(odsData.OrganisationName, 220), Parameter: nameof(OdsData.OrganisationName)),
                (Rule: IsInvalidLength(odsData.CreatedBy, 255), Parameter: nameof(OdsData.CreatedBy)),
                (Rule: IsInvalidLength(odsData.UpdatedBy, 255), Parameter: nameof(OdsData.UpdatedBy)),

                (Rule: IsNotSame(
                    first: currentUser.EntraUserId,
                    second: odsData.UpdatedBy),
                Parameter: nameof(OdsData.UpdatedBy)),

                (Rule: IsSameAs(
                    firstDate: odsData.CreatedDate,
                    secondDate: odsData.UpdatedDate,
                    secondDateName: nameof(OdsData.CreatedDate)),
                Parameter: nameof(OdsData.UpdatedDate)),

                (Rule: await IsNotRecentAsync(odsData.UpdatedDate), Parameter: nameof(OdsData.UpdatedDate))
            );
        }


        public static void ValidateOdsDataId(Guid odsDataId) =>
            Validate((Rule: IsInvalid(odsDataId), Parameter: nameof(OdsData.Id)));

        private static void ValidateStorageOdsData(OdsData maybeOdsData, Guid odsDataId)
        {
            if (maybeOdsData is null)
            {
                throw new NotFoundOdsDataException(message: $"OdsData not found with Id: {odsDataId}");
            }
        }

        private static void ValidateOdsDataIsNotNull(OdsData odsData)
        {
            if (odsData is null)
            {
                throw new NullOdsDataException(message: "OdsData is null.");
            }
        }

        private static void ValidateAgainstStorageOdsDataOnModify(OdsData inputOdsData, OdsData storageOdsData)
        { }

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
            DateTimeOffset? firstDate,
            DateTimeOffset? secondDate,
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
            var invalidOdsDataException =
                new InvalidOdsDataException(
                    message: "Invalid odsData. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidOdsDataException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidOdsDataException.ThrowIfContainsErrors();
        }
    }
}