// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;

namespace ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts
{
    public partial class ImpersonationContextService
    {
        private async ValueTask ValidateImpersonationContextOnAddAsync(ImpersonationContext impersonationContext)
        {
            ValidateImpersonationContextIsNotNull(impersonationContext);

            Validate(
                (Rule: IsInvalid(impersonationContext.Id),
                Parameter: nameof(ImpersonationContext.Id)),

                (Rule: IsInvalid(impersonationContext.RequesterEntraUserId),
                Parameter: nameof(ImpersonationContext.RequesterEntraUserId)),

                (Rule: IsInvalid(impersonationContext.RequesterEmail),
                Parameter: nameof(ImpersonationContext.RequesterEmail)),

                (Rule: IsInvalid(impersonationContext.ResponsiblePersonEntraUserId),
                Parameter: nameof(ImpersonationContext.ResponsiblePersonEntraUserId)),

                (Rule: IsInvalid(impersonationContext.ResponsiblePersonEmail),
                Parameter: nameof(ImpersonationContext.ResponsiblePersonEmail)),

                (Rule: IsInvalid(impersonationContext.IdentifierColumn),
                Parameter: nameof(ImpersonationContext.IdentifierColumn)),

                (Rule: IsInvalid(impersonationContext.ProjectName),
                Parameter: nameof(ImpersonationContext.ProjectName)),

                (Rule: IsInvalid(impersonationContext.CreatedBy),
                Parameter: nameof(ImpersonationContext.CreatedBy)),

                (Rule: IsInvalid(impersonationContext.UpdatedBy),
                Parameter: nameof(ImpersonationContext.UpdatedBy)),

                (Rule: IsInvalid(impersonationContext.CreatedDate),
                Parameter: nameof(ImpersonationContext.CreatedDate)),

                (Rule: IsInvalid(impersonationContext.UpdatedDate),
                Parameter: nameof(ImpersonationContext.UpdatedDate)),

                (Rule: IsInvalidLength(impersonationContext.ProjectName, 255),
                Parameter: nameof(ImpersonationContext.ProjectName)),

                (Rule: IsInvalidLength(impersonationContext.RequesterEmail, 320),
                Parameter: nameof(ImpersonationContext.RequesterEmail)),

                (Rule: IsInvalidLength(impersonationContext.ResponsiblePersonEmail, 320),
                Parameter: nameof(ImpersonationContext.ResponsiblePersonEmail)),

                (Rule: IsInvalidLength(impersonationContext.IdentifierColumn, 10),
                Parameter: nameof(ImpersonationContext.IdentifierColumn)),

                (Rule: IsInvalidLength(impersonationContext.CreatedBy, 255),
                Parameter: nameof(ImpersonationContext.CreatedBy)),

                (Rule: IsInvalidLength(impersonationContext.UpdatedBy, 255),
                Parameter: nameof(ImpersonationContext.UpdatedBy)),

                (Rule: IsNotSame(
                    first: impersonationContext.UpdatedBy,
                    second: impersonationContext.CreatedBy,
                    secondName: nameof(ImpersonationContext.CreatedBy)),

                Parameter: nameof(ImpersonationContext.UpdatedBy)),

                (Rule: IsNotSame(
                    first: impersonationContext.CreatedDate,
                    second: impersonationContext.UpdatedDate,
                    secondName: nameof(ImpersonationContext.CreatedDate)),

                Parameter: nameof(ImpersonationContext.UpdatedDate)),

                (Rule: await IsNotRecentAsync(impersonationContext.CreatedDate),
                Parameter: nameof(ImpersonationContext.CreatedDate)));
        }

        private async ValueTask ValidateImpersonationContextOnModifyAsync(ImpersonationContext impersonationContext)
        {
            ValidateImpersonationContextIsNotNull(impersonationContext);

            Validate(
                (Rule: IsInvalid(impersonationContext.Id), Parameter: nameof(ImpersonationContext.Id)),

                (Rule: IsInvalid(impersonationContext.RequesterEntraUserId),
                Parameter: nameof(ImpersonationContext.RequesterEntraUserId)),

                (Rule: IsInvalid(impersonationContext.RequesterEmail),
                Parameter: nameof(ImpersonationContext.RequesterEmail)),

                (Rule: IsInvalid(impersonationContext.ResponsiblePersonEntraUserId),
                Parameter: nameof(ImpersonationContext.ResponsiblePersonEntraUserId)),

                (Rule: IsInvalid(impersonationContext.ResponsiblePersonEmail),
                Parameter: nameof(ImpersonationContext.ResponsiblePersonEmail)),

                (Rule: IsInvalid(impersonationContext.IdentifierColumn),
                Parameter: nameof(ImpersonationContext.IdentifierColumn)),

                (Rule: IsInvalid(impersonationContext.ProjectName),
                Parameter: nameof(ImpersonationContext.ProjectName)),

                (Rule: IsInvalid(impersonationContext.CreatedBy),
                Parameter: nameof(ImpersonationContext.CreatedBy)),

                (Rule: IsInvalid(impersonationContext.UpdatedBy),
                Parameter: nameof(ImpersonationContext.UpdatedBy)),

                (Rule: IsInvalid(impersonationContext.CreatedDate),
                Parameter: nameof(ImpersonationContext.CreatedDate)),

                (Rule: IsInvalid(impersonationContext.UpdatedDate),
                Parameter: nameof(ImpersonationContext.UpdatedDate)),

                (Rule: IsInvalidLength(impersonationContext.ProjectName, 255),
                Parameter: nameof(ImpersonationContext.ProjectName)),

                (Rule: IsInvalidLength(impersonationContext.RequesterEmail, 320),
                Parameter: nameof(ImpersonationContext.RequesterEmail)),

                (Rule: IsInvalidLength(impersonationContext.ResponsiblePersonEmail, 320),
                Parameter: nameof(ImpersonationContext.ResponsiblePersonEmail)),

                (Rule: IsInvalidLength(impersonationContext.IdentifierColumn, 10),
                Parameter: nameof(ImpersonationContext.IdentifierColumn)),

                (Rule: IsInvalidLength(impersonationContext.CreatedBy, 255),
                Parameter: nameof(ImpersonationContext.CreatedBy)),

                (Rule: IsInvalidLength(impersonationContext.UpdatedBy, 255),
                Parameter: nameof(ImpersonationContext.UpdatedBy)),

                (Rule: IsSame(
                    firstDate: impersonationContext.UpdatedDate,
                    secondDate: impersonationContext.CreatedDate,
                    nameof(ImpersonationContext.CreatedDate)),

                Parameter: nameof(ImpersonationContext.UpdatedDate)),

                (Rule: await IsNotRecentAsync(impersonationContext.UpdatedDate),
                    Parameter: nameof(ImpersonationContext.UpdatedDate)));
        }

        private static void ValidateImpersonationContextId(Guid impersonationContextId) =>
            Validate((Rule: IsInvalid(impersonationContextId), Parameter: nameof(ImpersonationContext.Id)));

        private static void ValidateImpersonationContextIsNotNull(ImpersonationContext impersonationContext)
        {
            if (impersonationContext is null)
            {
                throw new NullImpersonationContextException("Impersonation context is null.");
            }
        }

        private static void ValidateStorageImpersonationContext(ImpersonationContext maybeImpersonationContext,
            Guid id)
        {
            if (maybeImpersonationContext is null)
            {
                throw new NotFoundImpersonationContextException(
                    message: $"Impersonation context not found with id: {id}");
            }
        }

        private static void ValidateAgainstStorageImpersonationContextOnModify(
            ImpersonationContext inputImpersonationContext, ImpersonationContext storageImpersonationContext)
        {
            Validate(
                (Rule: IsNotSame(
                    first: inputImpersonationContext.CreatedBy,
                    second: storageImpersonationContext.CreatedBy,
                    secondName: nameof(ImpersonationContext.CreatedBy)),

                Parameter: nameof(ImpersonationContext.CreatedBy)),

                (Rule: IsNotSame(
                    first: inputImpersonationContext.CreatedDate,
                    second: storageImpersonationContext.CreatedDate,
                    secondName: nameof(ImpersonationContext.CreatedDate)),

                Parameter: nameof(ImpersonationContext.CreatedDate)),

                (Rule: IsSame(
                    firstDate: inputImpersonationContext.UpdatedDate,
                    secondDate: storageImpersonationContext.UpdatedDate,
                    secondDateName: nameof(ImpersonationContext.UpdatedDate)),

                Parameter: nameof(ImpersonationContext.UpdatedDate)));
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

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
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
            var invalidImpersonationContextException =
                new InvalidImpersonationContextException(
                    message: "Invalid impersonation context. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidImpersonationContextException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidImpersonationContextException.ThrowIfContainsErrors();
        }
    }
}
