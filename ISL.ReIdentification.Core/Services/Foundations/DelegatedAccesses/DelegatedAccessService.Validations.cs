﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.DelegatedAccesses;
using ISL.ReIdentification.Core.Models.Foundations.DelegatedAccesses.Exceptions;

namespace ISL.ReIdentification.Core.Services.Foundations.DelegatedAccesses
{
    public partial class DelegatedAccessService
    {
        private async ValueTask ValidateDelegatedAccessOnAdd(DelegatedAccess delegatedAccess)
        {
            ValidateDelegatedAccessIsNotNull(delegatedAccess);

            Validate(
                (Rule: await IsInvalidAsync(delegatedAccess.Id), Parameter: nameof(DelegatedAccess.Id)),

                (Rule: await IsInvalidAsync(delegatedAccess.RequesterFirstName),
                Parameter: nameof(DelegatedAccess.RequesterFirstName)),

                (Rule: await IsInvalidAsync(delegatedAccess.RequesterLastName),
                Parameter: nameof(DelegatedAccess.RequesterLastName)),

                (Rule: await IsInvalidAsync(delegatedAccess.RequesterEmail),
                Parameter: nameof(DelegatedAccess.RequesterEmail)),

                (Rule: await IsInvalidAsync(delegatedAccess.RecipientFirstName),
                Parameter: nameof(DelegatedAccess.RecipientFirstName)),

                (Rule: await IsInvalidAsync(delegatedAccess.RecipientLastName),
                Parameter: nameof(DelegatedAccess.RecipientLastName)),

                (Rule: await IsInvalidAsync(delegatedAccess.RecipientEmail),
                Parameter: nameof(DelegatedAccess.RecipientEmail)),

                (Rule: await IsInvalidAsync(delegatedAccess.IdentifierColumn),
                Parameter: nameof(DelegatedAccess.IdentifierColumn)),

                (Rule: await IsInvalidAsync(delegatedAccess.CreatedBy), Parameter: nameof(DelegatedAccess.CreatedBy)),
                (Rule: await IsInvalidAsync(delegatedAccess.UpdatedBy), Parameter: nameof(DelegatedAccess.UpdatedBy)),

                (Rule: await IsInvalidAsync(delegatedAccess.CreatedDate),
                Parameter: nameof(DelegatedAccess.CreatedDate)),

                (Rule: await IsInvalidAsync(delegatedAccess.UpdatedDate),
                Parameter: nameof(DelegatedAccess.UpdatedDate)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.RequesterFirstName, 255),
                Parameter: nameof(DelegatedAccess.RequesterFirstName)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.RequesterLastName, 255),
                Parameter: nameof(DelegatedAccess.RequesterLastName)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.RequesterEmail, 320),
                Parameter: nameof(DelegatedAccess.RequesterEmail)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.RecipientFirstName, 255),
                Parameter: nameof(DelegatedAccess.RecipientFirstName)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.RecipientLastName, 255),
                Parameter: nameof(DelegatedAccess.RecipientLastName)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.RecipientEmail, 320),
                Parameter: nameof(DelegatedAccess.RecipientEmail)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.IdentifierColumn, 10),
                Parameter: nameof(DelegatedAccess.IdentifierColumn)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.CreatedBy, 255),
                Parameter: nameof(DelegatedAccess.CreatedBy)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.UpdatedBy, 255),
                Parameter: nameof(DelegatedAccess.UpdatedBy)),

                (Rule: await IsNotSameAsync(
                    first: delegatedAccess.UpdatedBy,
                    second: delegatedAccess.CreatedBy,
                    secondName: nameof(DelegatedAccess.CreatedBy)),

                Parameter: nameof(DelegatedAccess.UpdatedBy)),

                (Rule: await IsNotSameAsync(
                    first: delegatedAccess.CreatedDate,
                    second: delegatedAccess.UpdatedDate,
                    secondName: nameof(DelegatedAccess.CreatedDate)),

                Parameter: nameof(DelegatedAccess.UpdatedDate)),

                (Rule: await IsNotRecentAsync(delegatedAccess.CreatedDate),
                Parameter: nameof(DelegatedAccess.CreatedDate)));
        }

        private async ValueTask ValidateDelegatedAccessOnModify(DelegatedAccess delegatedAccess)
        {
            ValidateDelegatedAccessIsNotNull(delegatedAccess);

            Validate(
                (Rule: await IsInvalidAsync(delegatedAccess.Id), Parameter: nameof(DelegatedAccess.Id)),

                (Rule: await IsInvalidAsync(delegatedAccess.RequesterFirstName),
                Parameter: nameof(DelegatedAccess.RequesterFirstName)),

                (Rule: await IsInvalidAsync(delegatedAccess.RequesterLastName),
                Parameter: nameof(DelegatedAccess.RequesterLastName)),

                (Rule: await IsInvalidAsync(delegatedAccess.RequesterEmail),
                Parameter: nameof(DelegatedAccess.RequesterEmail)),

                (Rule: await IsInvalidAsync(delegatedAccess.RecipientFirstName),
                Parameter: nameof(DelegatedAccess.RecipientFirstName)),

                (Rule: await IsInvalidAsync(delegatedAccess.RecipientLastName),
                Parameter: nameof(DelegatedAccess.RecipientLastName)),

                (Rule: await IsInvalidAsync(delegatedAccess.RecipientEmail),
                Parameter: nameof(DelegatedAccess.RecipientEmail)),

                (Rule: await IsInvalidAsync(
                    delegatedAccess.IdentifierColumn),

                Parameter: nameof(DelegatedAccess.IdentifierColumn)),

                (Rule: await IsInvalidAsync(delegatedAccess.CreatedBy), Parameter: nameof(DelegatedAccess.CreatedBy)),
                (Rule: await IsInvalidAsync(delegatedAccess.UpdatedBy), Parameter: nameof(DelegatedAccess.UpdatedBy)),

                (Rule: await IsInvalidAsync(
                    delegatedAccess.CreatedDate),

                Parameter: nameof(DelegatedAccess.CreatedDate)),

                (Rule: await IsInvalidAsync(
                    delegatedAccess.UpdatedDate),

                Parameter: nameof(DelegatedAccess.UpdatedDate)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.RequesterFirstName, 255),
                Parameter: nameof(DelegatedAccess.RequesterFirstName)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.RequesterLastName, 255),
                Parameter: nameof(DelegatedAccess.RequesterLastName)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.RequesterEmail, 320),
                Parameter: nameof(DelegatedAccess.RequesterEmail)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.RecipientFirstName, 255),
                Parameter: nameof(DelegatedAccess.RecipientFirstName)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.RecipientLastName, 255),
                Parameter: nameof(DelegatedAccess.RecipientLastName)),

                (Rule: await IsInvalidLengthAsync(delegatedAccess.RecipientEmail, 320),
                Parameter: nameof(DelegatedAccess.RecipientEmail)),

                (Rule: await IsInvalidLengthAsync(
                    delegatedAccess.IdentifierColumn, 10),

                Parameter: nameof(DelegatedAccess.IdentifierColumn)),

                (Rule: await IsInvalidLengthAsync(
                    delegatedAccess.CreatedBy, 255),

                Parameter: nameof(DelegatedAccess.CreatedBy)),

                (Rule: await IsInvalidLengthAsync(
                    delegatedAccess.UpdatedBy, 255),

                Parameter: nameof(DelegatedAccess.UpdatedBy)),

                (Rule: await IsSameAsync(
                    firstDate: delegatedAccess.UpdatedDate,
                    secondDate: delegatedAccess.CreatedDate,
                    nameof(DelegatedAccess.CreatedDate)),

                Parameter: nameof(DelegatedAccess.UpdatedDate)),

                (Rule: await IsNotRecentAsync(delegatedAccess.UpdatedDate),
                    Parameter: nameof(DelegatedAccess.UpdatedDate)));
        }

        private static async ValueTask ValidateDelegatedAccessIdAsync(Guid delegatedAccessId) =>
            Validate((Rule: await IsInvalidAsync(delegatedAccessId), Parameter: nameof(DelegatedAccess.Id)));

        private static void ValidateDelegatedAccessIsNotNull(DelegatedAccess delegatedAccess)
        {
            if (delegatedAccess is null)
            {
                throw new NullDelegatedAccessException("Delegated access is null.");
            }
        }

        private static async ValueTask ValidateStorageDelegatedAccessAsync(DelegatedAccess maybeDelegatedAccess,
            Guid id)
        {
            if (maybeDelegatedAccess is null)
            {
                throw new NotFoundDelegatedAccessException(
                    message: $"DelegatedAccess not found with id: {id}");
            }
        }

        private static async ValueTask ValidateAgainstStorageDelegatedAccessOnModifyAsync(
            DelegatedAccess inputDelegatedAccess, DelegatedAccess storageDelegatedAccess)
        {
            Validate(
                (Rule: await IsNotSameAsync(
                    first: inputDelegatedAccess.CreatedBy,
                    second: storageDelegatedAccess.CreatedBy,
                    secondName: nameof(DelegatedAccess.CreatedBy)),

                Parameter: nameof(DelegatedAccess.CreatedBy)),

                (Rule: await IsNotSameAsync(
                    first: inputDelegatedAccess.CreatedDate,
                    second: storageDelegatedAccess.CreatedDate,
                    secondName: nameof(DelegatedAccess.CreatedDate)),

                Parameter: nameof(DelegatedAccess.CreatedDate)),

                (Rule: await IsSameAsync(
                    firstDate: inputDelegatedAccess.UpdatedDate,
                    secondDate: storageDelegatedAccess.UpdatedDate,
                    secondDateName: nameof(DelegatedAccess.UpdatedDate)),

                Parameter: nameof(DelegatedAccess.UpdatedDate)));
        }

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

        private static async ValueTask<dynamic> IsInvalidLengthAsync(string text, int maxLength) => new
        {
            Condition = await IsExceedingLengthAsync(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static async ValueTask<bool> IsExceedingLengthAsync(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static async ValueTask<dynamic> IsSameAsync(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static async ValueTask<dynamic> IsNotSameAsync(
            DateTimeOffset first,
            DateTimeOffset second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Date is not the same as {secondName}"
            };

        private static async ValueTask<dynamic> IsNotSameAsync(
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
            int pastSeconds = 60;
            int futureSeconds = 0;
            DateTimeOffset currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            if (currentDateTime == default)
            {
                return (false, default, default);
            }

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            DateTimeOffset startDate = currentDateTime.AddSeconds(-pastSeconds);
            DateTimeOffset endDate = currentDateTime.AddSeconds(futureSeconds);
            bool isNotRecent = timeDifference.TotalSeconds is > 60 or < 0;

            return (isNotRecent, startDate, endDate);
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidDelegatedAccessException =
                new InvalidDelegatedAccessException(
                    message: "Invalid delegated access. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDelegatedAccessException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDelegatedAccessException.ThrowIfContainsErrors();
        }
    }
}