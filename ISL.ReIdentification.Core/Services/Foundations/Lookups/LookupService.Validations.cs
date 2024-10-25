// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Extensions.Loggings;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions;

namespace ISL.ReIdentification.Core.Services.Foundations.Lookups
{
    public partial class LookupService
    {
        private async ValueTask ValidateLookupOnAddAsync(Lookup lookup)
        {
            ValidateLookupIsNotNull(lookup);

            Validate(
                (Rule: IsInvalid(lookup.Id), Parameter: nameof(Lookup.Id)),
                (Rule: IsInvalid(lookup.Name), Parameter: nameof(Lookup.Name)),
                (Rule: IsInvalid(lookup.CreatedDate), Parameter: nameof(Lookup.CreatedDate)),
                (Rule: IsInvalid(lookup.CreatedBy), Parameter: nameof(Lookup.CreatedBy)),
                (Rule: IsInvalid(lookup.UpdatedDate), Parameter: nameof(Lookup.UpdatedDate)),
                (Rule: IsInvalid(lookup.UpdatedBy), Parameter: nameof(Lookup.UpdatedBy)),
                (Rule: IsInvalidLength(lookup.Name, 450), Parameter: nameof(Lookup.Name)),
                (Rule: IsInvalidLength(lookup.CreatedBy, 255), Parameter: nameof(Lookup.CreatedBy)),
                (Rule: IsInvalidLength(lookup.UpdatedBy, 255), Parameter: nameof(Lookup.UpdatedBy)),

                (Rule: IsNotSame(
                    first: lookup.UpdatedBy,
                    second: lookup.CreatedBy,
                    secondName: nameof(Lookup.CreatedBy)),

                Parameter: nameof(Lookup.UpdatedBy)),

                (Rule: IsNotSame(
                    first: lookup.UpdatedDate,
                    second: lookup.CreatedDate,
                    secondName: nameof(Lookup.CreatedDate)),

                Parameter: nameof(Lookup.UpdatedDate)),

                (Rule: await IsNotRecentAsync(lookup.CreatedDate), Parameter: nameof(Lookup.CreatedDate)));
        }

        private async ValueTask ValidateLookupOnModifyAsync(Lookup lookup)
        {
            ValidateLookupIsNotNull(lookup);

            Validate(
                (Rule: IsInvalid(lookup.Id), Parameter: nameof(Lookup.Id)),
                (Rule: IsInvalid(lookup.Name), Parameter: nameof(Lookup.Name)),
                (Rule: IsInvalid(lookup.CreatedDate), Parameter: nameof(Lookup.CreatedDate)),
                (Rule: IsInvalid(lookup.CreatedBy), Parameter: nameof(Lookup.CreatedBy)),
                (Rule: IsInvalid(lookup.UpdatedDate), Parameter: nameof(Lookup.UpdatedDate)),
                (Rule: IsInvalid(lookup.UpdatedBy), Parameter: nameof(Lookup.UpdatedBy)),
                (Rule: IsInvalidLength(lookup.Name, 450), Parameter: nameof(Lookup.Name)),
                (Rule: IsInvalidLength(lookup.CreatedBy, 255), Parameter: nameof(Lookup.CreatedBy)),
                (Rule: IsInvalidLength(lookup.UpdatedBy, 255), Parameter: nameof(Lookup.UpdatedBy)),

                (Rule: IsSameAs(
                    firstDate: lookup.UpdatedDate,
                    secondDate: lookup.CreatedDate,
                    secondDateName: nameof(Lookup.CreatedDate)),
                Parameter: nameof(Lookup.UpdatedDate)),

                (Rule: await IsNotRecentAsync(lookup.UpdatedDate), Parameter: nameof(lookup.UpdatedDate)));
        }

        public static void ValidateLookupId(Guid lookupId) =>
            Validate((Rule: IsInvalid(lookupId), Parameter: nameof(Lookup.Id)));

        private static void ValidateStorageLookup(Lookup maybeLookup, Guid lookupId)
        {
            if (maybeLookup is null)
            {
                throw new NotFoundLookupException(message: $"Lookup not found with Id: {lookupId}");
            }
        }

        private static void ValidateLookupIsNotNull(Lookup lookup)
        {
            if (lookup is null)
            {
                throw new NullLookupException(message: "Lookup is null.");
            }
        }

        private static void ValidateAgainstStorageLookupOnModify(Lookup inputLookup, Lookup storageLookup)
        {
            Validate(
                (Rule: IsNotSame(
                    first: inputLookup.CreatedBy,
                    second: storageLookup.CreatedBy,
                    secondName: nameof(Lookup.CreatedBy)),
                Parameter: nameof(Lookup.CreatedBy)),

                (Rule: IsNotSame(
                    first: inputLookup.CreatedDate,
                    second: storageLookup.CreatedDate,
                    secondName: nameof(Lookup.CreatedDate)),
                Parameter: nameof(Lookup.CreatedDate)),

                (Rule: IsSameAs(
                    firstDate: inputLookup.UpdatedDate,
                    secondDate: storageLookup.UpdatedDate,
                    secondDateName: nameof(Lookup.UpdatedDate)),
                Parameter: nameof(Lookup.UpdatedDate)));
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
            int pastThreshold = 60;
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
            var invalidLookupException =
                new InvalidLookupException(
                    message: "Invalid lookup. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidLookupException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            if (invalidLookupException.Data.Count > 0)
            {
                Console.WriteLine($"VALIDATION ERROR: {invalidLookupException.GetValidationSummary()}");
            }

            invalidLookupException.ThrowIfContainsErrors();
        }
    }
}