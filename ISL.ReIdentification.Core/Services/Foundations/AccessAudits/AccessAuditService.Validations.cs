// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;

namespace ISL.ReIdentification.Core.Services.Foundations.AccessAudits
{
    public partial class AccessAuditService
    {
        private async ValueTask ValidateAccessAuditOnAddAsync(AccessAudit accessAudit)
        {
            ValidateAccessAuditIsNotNull(accessAudit);
            EntraUser currentUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: await IsInvalidAsync(accessAudit.Id), Parameter: nameof(AccessAudit.Id)),
                (Rule: await IsInvalidAsync(accessAudit.RequestId), Parameter: nameof(AccessAudit.RequestId)),
                (Rule: await IsInvalidAsync(accessAudit.EntraUserId), Parameter: nameof(AccessAudit.EntraUserId)),

                (Rule: await IsInvalidAsync(accessAudit.PseudoIdentifier),
                Parameter: nameof(AccessAudit.PseudoIdentifier)),

                (Rule: await IsInvalidAsync(accessAudit.AuditType), Parameter: nameof(AccessAudit.AuditType)),
                (Rule: await IsInvalidAsync(accessAudit.Email), Parameter: nameof(AccessAudit.Email)),
                (Rule: await IsInvalidAsync(accessAudit.CreatedBy), Parameter: nameof(AccessAudit.CreatedBy)),
                (Rule: await IsInvalidAsync(accessAudit.UpdatedBy), Parameter: nameof(AccessAudit.UpdatedBy)),
                (Rule: await IsInvalidAsync(accessAudit.CreatedDate), Parameter: nameof(AccessAudit.CreatedDate)),
                (Rule: await IsInvalidAsync(accessAudit.UpdatedDate), Parameter: nameof(AccessAudit.UpdatedDate)),

                (Rule: await IsInvalidLengthAsync(accessAudit.EntraUserId, 255),
                Parameter: nameof(AccessAudit.EntraUserId)),

                (Rule: await IsInvalidLengthAsync(accessAudit.AuditType, 255), Parameter: nameof(AccessAudit.AuditType)),
                (Rule: await IsInvalidLengthAsync(accessAudit.CreatedBy, 255), Parameter: nameof(AccessAudit.CreatedBy)),
                (Rule: await IsInvalidLengthAsync(accessAudit.UpdatedBy, 255), Parameter: nameof(AccessAudit.UpdatedBy)),
                (Rule: await IsInvalidLengthAsync(accessAudit.Email, 320), Parameter: nameof(AccessAudit.Email)),

                (Rule: await IsInvalidLengthAsync(accessAudit.PseudoIdentifier, 10),
                Parameter: nameof(AccessAudit.PseudoIdentifier)),

                (Rule: IsNotSame(
                    first: currentUser.EntraUserId,
                    second: accessAudit.CreatedBy),
                Parameter: nameof(AccessAudit.CreatedBy)),

                (Rule: await IsNotSameAsync(
                    first: accessAudit.UpdatedBy,
                    second: accessAudit.CreatedBy,
                    secondName: nameof(AccessAudit.CreatedBy)),
                Parameter: nameof(AccessAudit.UpdatedBy)),

                (Rule: await IsNotSameAsync(
                    first: accessAudit.UpdatedDate,
                    second: accessAudit.CreatedDate,
                    secondName: nameof(AccessAudit.CreatedDate)),
                Parameter: nameof(AccessAudit.UpdatedDate)),

                (Rule: await IsNotRecentAsync(accessAudit.CreatedDate), Parameter: nameof(AccessAudit.CreatedDate)));
        }

        private async ValueTask ValidateAccessAuditOnBulkAddAsync(List<AccessAudit> accessAudits)
        {
            var exceptions = new List<Exception>();

            foreach (AccessAudit accessAudit in accessAudits)
            {
                try
                {
                    await ValidateAccessAuditOnAddAsync(accessAudit);
                }
                catch (Exception exception)
                {
                    exceptions.Add(exception);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(
                    $"Unable to validate access for {exceptions.Count} audit access requests.",
                    exceptions);
            }
        }

        private async ValueTask ValidateAccessAuditOnRetrieveById(Guid accessAuditId) =>
            Validate((Rule: await IsInvalidAsync(accessAuditId), Parameter: nameof(AccessAudit.Id)));

        private async ValueTask ValidateAccessAuditOnModifyAsync(AccessAudit accessAudit)
        {
            ValidateAccessAuditIsNotNull(accessAudit);
            EntraUser currentUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: await IsInvalidAsync(accessAudit.Id), Parameter: nameof(AccessAudit.Id)),
                (Rule: await IsInvalidAsync(accessAudit.RequestId), Parameter: nameof(AccessAudit.RequestId)),
                (Rule: await IsInvalidAsync(accessAudit.EntraUserId), Parameter: nameof(AccessAudit.EntraUserId)),

                (Rule: await IsInvalidAsync(accessAudit.PseudoIdentifier),
                Parameter: nameof(AccessAudit.PseudoIdentifier)),

                (Rule: await IsInvalidAsync(accessAudit.AuditType), Parameter: nameof(AccessAudit.AuditType)),
                (Rule: await IsInvalidAsync(accessAudit.Email), Parameter: nameof(AccessAudit.Email)),
                (Rule: await IsInvalidAsync(accessAudit.CreatedBy), Parameter: nameof(AccessAudit.CreatedBy)),
                (Rule: await IsInvalidAsync(accessAudit.UpdatedBy), Parameter: nameof(AccessAudit.UpdatedBy)),
                (Rule: await IsInvalidAsync(accessAudit.CreatedDate), Parameter: nameof(AccessAudit.CreatedDate)),
                (Rule: await IsInvalidAsync(accessAudit.UpdatedDate), Parameter: nameof(AccessAudit.UpdatedDate)),

                (Rule: await IsInvalidLengthAsync(accessAudit.EntraUserId, 255),
                Parameter: nameof(AccessAudit.EntraUserId)),

                (Rule: await IsInvalidLengthAsync(accessAudit.AuditType, 255), Parameter: nameof(AccessAudit.AuditType)),
                (Rule: await IsInvalidLengthAsync(accessAudit.CreatedBy, 255), Parameter: nameof(AccessAudit.CreatedBy)),
                (Rule: await IsInvalidLengthAsync(accessAudit.UpdatedBy, 255), Parameter: nameof(AccessAudit.UpdatedBy)),
                (Rule: await IsInvalidLengthAsync(accessAudit.Email, 320), Parameter: nameof(AccessAudit.Email)),

                (Rule: await IsInvalidLengthAsync(accessAudit.PseudoIdentifier, 10),
                Parameter: nameof(AccessAudit.PseudoIdentifier)),

                (Rule: IsNotSame(
                    first: currentUser.EntraUserId,
                    second: accessAudit.UpdatedBy),
                Parameter: nameof(AccessAudit.UpdatedBy)),

                (Rule: IsSameAs(
                    createdDate: accessAudit.CreatedDate,
                    updatedDate: accessAudit.UpdatedDate,
                    createdDateName: nameof(AccessAudit.CreatedDate)),
                Parameter: nameof(AccessAudit.UpdatedDate)),

                (Rule: await IsNotRecentAsync(accessAudit.UpdatedDate), Parameter: nameof(AccessAudit.UpdatedDate)));
        }

        private async ValueTask ValidateAccessAuditOnRemoveById(Guid accessAuditId) =>
            Validate((Rule: await IsInvalidAsync(accessAuditId), Parameter: nameof(AccessAudit.Id)));

        private static void ValidateAccessAuditIsNotNull(AccessAudit accessAudit)
        {
            if (accessAudit is null)
            {
                throw new NullAccessAuditException("Access audit is null.");
            }
        }

        private static void ValidateAccessAuditIsNotNull(List<AccessAudit> accessAudits)
        {
            if (accessAudits is null)
            {
                throw new NullAccessAuditException("Access audit list is null.");
            }
        }

        private static async ValueTask ValidateStorageAccessAuditAsync(AccessAudit maybeAccessAudit, Guid maybeId)
        {
            if (maybeAccessAudit is null)
            {
                throw new NotFoundAccessAuditException($"Access audit not found with Id: {maybeId}");
            }
        }

        private async ValueTask ValidateAgainstStorageAccessAuditOnModifyAsync(
            AccessAudit accessAudit,
            AccessAudit maybeAccessAudit)
        {
            Validate(
                (Rule: await IsNotSameAsync(
                    accessAudit.CreatedDate,
                    maybeAccessAudit.CreatedDate,
                    nameof(maybeAccessAudit.CreatedDate)),

                Parameter: nameof(AccessAudit.CreatedDate)),

                (Rule: IsSameAs(
                    accessAudit.UpdatedDate,
                    maybeAccessAudit.UpdatedDate,
                    nameof(maybeAccessAudit.UpdatedDate)),

                Parameter: nameof(AccessAudit.UpdatedDate)));
        }

        private async ValueTask ValidateAgainstStorageAccessAuditOnDeleteAsync(AccessAudit accessAudit, AccessAudit maybeAccessAudit)
        {
            EntraUser auditUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: IsNotSame(
                    accessAudit.CreatedDate,
                    maybeAccessAudit.CreatedDate,
                    nameof(maybeAccessAudit.CreatedDate)),
                 Parameter: nameof(AccessAudit.CreatedDate)),

                (Rule: await IsNotSameAsync(
                    accessAudit.CreatedBy,
                    maybeAccessAudit.CreatedBy,
                    nameof(maybeAccessAudit.CreatedBy)),
                 Parameter: nameof(AccessAudit.CreatedBy)),

                (Rule: IsNotSame(
                    maybeAccessAudit.UpdatedDate,
                    accessAudit.UpdatedDate,
                    nameof(AccessAudit.UpdatedDate)),
                 Parameter: nameof(AccessAudit.UpdatedDate)),

                (Rule: await IsNotSameAsync(
                    auditUser.EntraUserId.ToString(),
                    accessAudit.UpdatedBy,
                    nameof(AccessAudit.UpdatedBy)),
                 Parameter: nameof(AccessAudit.UpdatedBy))
            );
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

        private static dynamic IsNotSame(
            string first,
            string second) => new
            {
                Condition = first != second,
                Message = $"Expected value to be '{first}' but found '{second}'."
            };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static async ValueTask<dynamic> IsNotSameAsync(
            string first,
            string second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Text is not the same as {secondName}"
            };

        private static dynamic IsSameAs(
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
            var invalidAccessAuditException =
                new InvalidAccessAuditException(
                    message: "Invalid access audit. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidAccessAuditException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidAccessAuditException.ThrowIfContainsErrors();
        }
    }
}
