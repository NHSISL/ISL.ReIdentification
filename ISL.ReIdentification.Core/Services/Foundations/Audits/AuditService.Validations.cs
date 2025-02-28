// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using ISL.ReIdentification.Core.Models.Foundations.Audits.Exceptions;

namespace ISL.ReIdentification.Core.Services.Foundations.Audits
{
    public partial class AuditService
    {
        private async ValueTask ValidateAuditOnAddAsync(Audit audit)
        {
            ValidateAuditIsNotNull(audit);

            Validate(
                (Rule: await IsInvalidAsync(audit.Id), Parameter: nameof(Audit.Id)),
                (Rule: await IsInvalidAsync(audit.AuditType), Parameter: nameof(Audit.AuditType)),
                (Rule: await IsInvalidAsync(audit.AuditDetail), Parameter: nameof(Audit.AuditDetail)),
                (Rule: await IsInvalidAsync(audit.LogLevel), Parameter: nameof(Audit.LogLevel)),
                (Rule: await IsInvalidAsync(audit.CreatedBy), Parameter: nameof(Audit.CreatedBy)),
                (Rule: await IsInvalidAsync(audit.UpdatedBy), Parameter: nameof(Audit.UpdatedBy)),
                (Rule: await IsInvalidAsync(audit.CreatedDate), Parameter: nameof(Audit.CreatedDate)),
                (Rule: await IsInvalidAsync(audit.UpdatedDate), Parameter: nameof(Audit.UpdatedDate)),
                (Rule: await IsInvalidLengthAsync(audit.AuditType, 255), Parameter: nameof(Audit.AuditType)),
                (Rule: await IsInvalidLengthAsync(audit.LogLevel, 255), Parameter: nameof(Audit.LogLevel)),
                (Rule: await IsInvalidLengthAsync(audit.CreatedBy, 255), Parameter: nameof(Audit.CreatedBy)),
                (Rule: await IsInvalidLengthAsync(audit.UpdatedBy, 255), Parameter: nameof(Audit.UpdatedBy)),

                (Rule: await IsNotSameAsync(
                    first: audit.UpdatedBy,
                    second: audit.CreatedBy,
                    secondName: nameof(Audit.CreatedBy)),

                Parameter: nameof(Audit.UpdatedBy)),

                (Rule: await IsNotSameAsync(
                    first: audit.UpdatedDate,
                    second: audit.CreatedDate,
                    secondName: nameof(Audit.CreatedDate)),

                Parameter: nameof(Audit.UpdatedDate)),

                (Rule: await IsNotRecentAsync(audit.CreatedDate), Parameter: nameof(Audit.CreatedDate)));
        }

        private async ValueTask ValidateAuditOnRetrieveById(Guid auditId) =>
            Validate((Rule: await IsInvalidAsync(auditId), Parameter: nameof(Audit.Id)));

        private async ValueTask ValidateAuditOnRetrieveByAuditType(string auditType) =>
           Validate((Rule: await IsInvalidAsync(auditType), Parameter: nameof(Audit.AuditType)));

        private async ValueTask ValidateAuditOnModifyAsync(Audit audit)
        {
            ValidateAuditIsNotNull(audit);

            Validate(
                (Rule: await IsInvalidAsync(audit.Id), Parameter: nameof(Audit.Id)),
                (Rule: await IsInvalidAsync(audit.AuditType), Parameter: nameof(Audit.AuditType)),
                (Rule: await IsInvalidAsync(audit.AuditDetail), Parameter: nameof(Audit.AuditDetail)),
                (Rule: await IsInvalidAsync(audit.LogLevel), Parameter: nameof(Audit.LogLevel)),
                (Rule: await IsInvalidAsync(audit.CreatedBy), Parameter: nameof(Audit.CreatedBy)),
                (Rule: await IsInvalidAsync(audit.UpdatedBy), Parameter: nameof(Audit.UpdatedBy)),
                (Rule: await IsInvalidAsync(audit.CreatedDate), Parameter: nameof(Audit.CreatedDate)),
                (Rule: await IsInvalidAsync(audit.UpdatedDate), Parameter: nameof(Audit.UpdatedDate)),
                (Rule: await IsInvalidLengthAsync(audit.AuditType, 255), Parameter: nameof(Audit.AuditType)),
                (Rule: await IsInvalidLengthAsync(audit.LogLevel, 255), Parameter: nameof(Audit.LogLevel)),
                (Rule: await IsInvalidLengthAsync(audit.CreatedBy, 255), Parameter: nameof(Audit.CreatedBy)),
                (Rule: await IsInvalidLengthAsync(audit.UpdatedBy, 255), Parameter: nameof(Audit.UpdatedBy)),

                (Rule: await IsSameAsAsync(
                    createdDate: audit.CreatedDate,
                    updatedDate: audit.UpdatedDate,
                    createdDateName: nameof(Audit.CreatedDate)),

                Parameter: nameof(Audit.UpdatedDate)),

                (Rule: await IsNotRecentAsync(audit.UpdatedDate), Parameter: nameof(Audit.UpdatedDate)));
        }

        private async ValueTask ValidateAuditOnRemoveById(Guid auditId) =>
            Validate((Rule: await IsInvalidAsync(auditId), Parameter: nameof(Audit.Id)));

        private static void ValidateAuditIsNotNull(Audit audit)
        {
            if (audit is null)
            {
                throw new NullAuditException("Audit is null.");
            }
        }

        private static async ValueTask ValidateStorageAuditAsync(Audit maybeAudit, Guid maybeId)
        {
            if (maybeAudit is null)
            {
                throw new NotFoundAuditException($"Audit not found with Id: {maybeId}");
            }
        }

        private static async ValueTask ValidateStorageAuditTypeAsync(Audit maybeAudit, string maybeAuditType)
        {
            if (maybeAudit is null)
            {
                throw new NotFoundAuditException($"Audit not found with audit Type: {maybeAuditType}");
            }
        }

        private async ValueTask ValidateAgainstStorageAuditOnModifyAsync(
            Audit audit,
            Audit maybeAudit)
        {
            Validate(
                (Rule: await IsNotSameAsync(
                    audit.CreatedDate,
                    maybeAudit.CreatedDate,
                    nameof(maybeAudit.CreatedDate)),

                Parameter: nameof(Audit.CreatedDate)),

                (Rule: await IsSameAsAsync(
                    audit.UpdatedDate,
                    maybeAudit.UpdatedDate,
                    nameof(maybeAudit.UpdatedDate)),

                Parameter: nameof(Audit.UpdatedDate)));
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

        private static async ValueTask<dynamic> IsSameAsAsync(
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
            var invalidAuditException =
                new InvalidAuditException(
                    message: "Invalid audit. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidAuditException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidAuditException.ThrowIfContainsErrors();
        }
    }
}
