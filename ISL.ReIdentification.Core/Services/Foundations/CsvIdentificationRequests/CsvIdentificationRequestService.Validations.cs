// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;

namespace ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestService
    {
        private async ValueTask ValidateCsvIdentificationRequestOnAdd(
            CsvIdentificationRequest csvIdentificationRequest)
        {
            ValidateCsvIdentificationRequestIsNotNull(csvIdentificationRequest);
            EntraUser currentUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: await IsInvalidAsync(csvIdentificationRequest.Id),
                Parameter: nameof(CsvIdentificationRequest.Id)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.RequesterEntraUserId),
                Parameter: nameof(CsvIdentificationRequest.RequesterEntraUserId)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.RequesterEmail),
                Parameter: nameof(CsvIdentificationRequest.RequesterEmail)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.RecipientEntraUserId),
                Parameter: nameof(CsvIdentificationRequest.RecipientEntraUserId)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.RecipientEmail),
                Parameter: nameof(CsvIdentificationRequest.RecipientEmail)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.Filepath),
                Parameter: nameof(CsvIdentificationRequest.Filepath)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.CreatedBy),
                Parameter: nameof(CsvIdentificationRequest.CreatedBy)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.UpdatedBy),
                Parameter: nameof(CsvIdentificationRequest.UpdatedBy)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.CreatedDate),
                Parameter: nameof(CsvIdentificationRequest.CreatedDate)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.UpdatedDate),
                Parameter: nameof(CsvIdentificationRequest.UpdatedDate)),

                (Rule: await IsInvalidLengthAsync(csvIdentificationRequest.RequesterEmail, 320),
                Parameter: nameof(CsvIdentificationRequest.RequesterEmail)),

                (Rule: await IsInvalidLengthAsync(csvIdentificationRequest.RecipientEmail, 320),
                Parameter: nameof(CsvIdentificationRequest.RecipientEmail)),

                (Rule: await IsInvalidLengthAsync(csvIdentificationRequest.CreatedBy, 255),
                Parameter: nameof(CsvIdentificationRequest.CreatedBy)),

                (Rule: await IsInvalidLengthAsync(csvIdentificationRequest.UpdatedBy, 255),
                Parameter: nameof(CsvIdentificationRequest.UpdatedBy)),

                (Rule: IsNotSame(
                    first: currentUser.EntraUserId,
                    second: csvIdentificationRequest.CreatedBy),
                Parameter: nameof(CsvIdentificationRequest.CreatedBy)),

                (Rule: await IsNotSameAsync(
                    first: csvIdentificationRequest.UpdatedBy,
                    second: csvIdentificationRequest.CreatedBy,
                    secondName: nameof(CsvIdentificationRequest.CreatedBy)),
                Parameter: nameof(CsvIdentificationRequest.UpdatedBy)),

                (Rule: await IsNotSameAsync(
                    first: csvIdentificationRequest.UpdatedDate,
                    second: csvIdentificationRequest.CreatedDate,
                    secondName: nameof(CsvIdentificationRequest.CreatedDate)),
                Parameter: nameof(CsvIdentificationRequest.UpdatedDate)),

                (Rule: await IsNotRecentAsync(csvIdentificationRequest.CreatedDate),
                Parameter: nameof(CsvIdentificationRequest.CreatedDate)));
        }

        private async ValueTask ValidateCsvIdentificationRequestOnModify(
            CsvIdentificationRequest csvIdentificationRequest)
        {
            ValidateCsvIdentificationRequestIsNotNull(csvIdentificationRequest);
            EntraUser currentUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: await IsInvalidAsync(csvIdentificationRequest.Id),
                Parameter: nameof(CsvIdentificationRequest.Id)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.RequesterEntraUserId),
                Parameter: nameof(CsvIdentificationRequest.RequesterEntraUserId)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.RequesterEmail),
                Parameter: nameof(CsvIdentificationRequest.RequesterEmail)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.RecipientEntraUserId),
                Parameter: nameof(CsvIdentificationRequest.RecipientEntraUserId)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.RecipientEmail),
                Parameter: nameof(CsvIdentificationRequest.RecipientEmail)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.Filepath),
                Parameter: nameof(CsvIdentificationRequest.Filepath)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.CreatedBy),
                Parameter: nameof(CsvIdentificationRequest.CreatedBy)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.UpdatedBy),
                Parameter: nameof(CsvIdentificationRequest.UpdatedBy)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.CreatedDate),
                Parameter: nameof(CsvIdentificationRequest.CreatedDate)),

                (Rule: await IsInvalidAsync(csvIdentificationRequest.UpdatedDate),
                Parameter: nameof(CsvIdentificationRequest.UpdatedDate)),

                (Rule: await IsInvalidLengthAsync(csvIdentificationRequest.RequesterEmail, 320),
                Parameter: nameof(CsvIdentificationRequest.RequesterEmail)),

                (Rule: await IsInvalidLengthAsync(csvIdentificationRequest.RecipientEmail, 320),
                Parameter: nameof(CsvIdentificationRequest.RecipientEmail)),

                (Rule: await IsInvalidLengthAsync(csvIdentificationRequest.CreatedBy, 255),
                Parameter: nameof(CsvIdentificationRequest.CreatedBy)),

                (Rule: await IsInvalidLengthAsync(csvIdentificationRequest.UpdatedBy, 255),
                Parameter: nameof(CsvIdentificationRequest.UpdatedBy)),

                (Rule: IsNotSame(
                    first: currentUser.EntraUserId,
                    second: csvIdentificationRequest.UpdatedBy),
                Parameter: nameof(CsvIdentificationRequest.UpdatedBy)),

                (Rule: await IsSameAsAsync(
                    createdDate: csvIdentificationRequest.CreatedDate,
                    updatedDate: csvIdentificationRequest.UpdatedDate,
                    createdDateName: nameof(CsvIdentificationRequest.CreatedDate)),
                Parameter: nameof(CsvIdentificationRequest.UpdatedDate)),

                (Rule: await IsNotRecentAsync(csvIdentificationRequest.UpdatedDate),
                    Parameter: nameof(CsvIdentificationRequest.UpdatedDate)));
        }

        private static void ValidateCsvIdentificationRequestId(Guid csvIdentificationRequestId)
        {
            Validate(
                (Rule: IsInvalid(csvIdentificationRequestId),
                Parameter: nameof(CsvIdentificationRequest.Id)));
        }

        private static void ValidateCsvIdentificationRequestIsNotNull(CsvIdentificationRequest csvIdentificationRequest)
        {
            if (csvIdentificationRequest is null)
            {
                throw new NullCsvIdentificationRequestException("CSV identification request is null.");
            }
        }

        private static void ValidateStorageCsvIdentificationRequest(CsvIdentificationRequest maybeCsvIdentificationRequest,
            Guid id)
        {
            if (maybeCsvIdentificationRequest is null)
            {
                throw new NotFoundCsvIdentificationRequestException(
                    message: $"CSV identification request not found with id: {id}");
            }
        }

        private static void ValidateAgainstStorageCsvIdentificationRequestOnModify(
            CsvIdentificationRequest inputCsvIdentificationRequest, CsvIdentificationRequest storageCsvIdentificationRequest)
        {
            Validate(
                (Rule: IsNotSame(
                    first: inputCsvIdentificationRequest.CreatedBy,
                    second: storageCsvIdentificationRequest.CreatedBy,
                    secondName: nameof(CsvIdentificationRequest.CreatedBy)),

                Parameter: nameof(CsvIdentificationRequest.CreatedBy)),

                (Rule: IsNotSame(
                    first: inputCsvIdentificationRequest.CreatedDate,
                    second: storageCsvIdentificationRequest.CreatedDate,
                    secondName: nameof(CsvIdentificationRequest.CreatedDate)),

                Parameter: nameof(CsvIdentificationRequest.CreatedDate)),

                (Rule: IsSame(
                    firstDate: inputCsvIdentificationRequest.UpdatedDate,
                    secondDate: storageCsvIdentificationRequest.UpdatedDate,
                    secondDateName: nameof(CsvIdentificationRequest.UpdatedDate)),

                Parameter: nameof(CsvIdentificationRequest.UpdatedDate)));
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

        private static async ValueTask<dynamic> IsInvalidAsync(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        private static async ValueTask<dynamic> IsInvalidAsync(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Text is invalid"
        };

        private static async ValueTask<dynamic> IsInvalidAsync(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is invalid"
        };

        private static dynamic IsInvalidLength(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static async ValueTask<dynamic> IsInvalidLengthAsync(string text, int maxLength) => new
        {
            Condition = await IsExceedingLengthAsync(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static async ValueTask<bool> IsExceedingLengthAsync(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static async ValueTask<dynamic> IsSameAsAsync(
            DateTimeOffset createdDate,
            DateTimeOffset updatedDate,
            string createdDateName) => new
            {
                Condition = createdDate == updatedDate,
                Message = $"Date is the same as {createdDateName}"
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
                Message = $"Expected value to be '{first}' but found '{second}'."
            };

        private static async ValueTask<dynamic> IsNotSameAsync(
            string first,
            string second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Text is not the same as {secondName}"
            };

        private static async ValueTask<dynamic> IsNotSameAsync(
            DateTimeOffset first,
            DateTimeOffset second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Date is not the same as {secondName}"
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
            var invalidCsvIdentificationRequestException =
                new InvalidCsvIdentificationRequestException(
                    message: "Invalid CSV identification request. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidCsvIdentificationRequestException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidCsvIdentificationRequestException.ThrowIfContainsErrors();
        }
    }
}
