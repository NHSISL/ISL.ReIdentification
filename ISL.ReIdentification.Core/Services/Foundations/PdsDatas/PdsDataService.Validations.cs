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
        private static void ValidatePdsDataOnAdd(PdsData pdsData)
        {
            ValidatePdsDataIsNotNull(pdsData);

            Validate(
                (Rule: IsInvalid(pdsData.Id), Parameter: nameof(PdsData.Id)),
                (Rule: IsInvalid(pdsData.PseudoNhsNumber), Parameter: nameof(PdsData.PseudoNhsNumber)));
        }

        private static void ValidatePdsDataOnModify(PdsData pdsData)
        {
            ValidatePdsDataIsNotNull(pdsData);

            Validate(
                (Rule: IsInvalid(pdsData.Id), Parameter: nameof(PdsData.Id)),
                (Rule: IsInvalid(pdsData.PseudoNhsNumber), Parameter: nameof(PdsData.PseudoNhsNumber)));
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
        {
            Validate(
                (Rule: IsNotSame(
                    firstDate: inputPdsData.CreatedDate,
                    secondDate: storagePdsData.CreatedDate,
                    secondDateName: nameof(PdsData.CreatedDate)),
                Parameter: nameof(PdsData.CreatedDate)),

                (Rule: IsNotSame(
                    first: inputPdsData.CreatedBy,
                    second: storagePdsData.CreatedBy,
                    secondName: nameof(PdsData.CreatedBy)),
                Parameter: nameof(PdsData.CreatedBy)),

                (Rule: IsSameAs(
                    firstDate: inputPdsData.UpdatedDate,
                    secondDate: storagePdsData.UpdatedDate,
                    secondDateName: nameof(PdsData.UpdatedDate)),
                Parameter: nameof(PdsData.UpdatedDate)));
        }

        private async ValueTask ValidateAgainstStoragePdsDataOnDeleteAsync(
            PdsData pdsData,
            PdsData maybePdsData)
        {
            EntraUser auditUser = await this.securityBroker.GetCurrentUserAsync();

            Validate(
                (Rule: IsNotSame(
                    pdsData.CreatedDate,
                    maybePdsData.CreatedDate,
                    nameof(maybePdsData.CreatedDate)),
                 Parameter: nameof(PdsData.CreatedDate)),

                (Rule: IsNotSame(
            pdsData.CreatedBy,
                    maybePdsData.CreatedBy,
                    nameof(maybePdsData.CreatedBy)),
                 Parameter: nameof(PdsData.CreatedBy)),
            (Rule: IsNotSame(
                    maybePdsData.UpdatedDate,
                    pdsData.UpdatedDate,
                    nameof(PdsData.UpdatedDate)),
                 Parameter: nameof(PdsData.UpdatedDate)),

                (Rule: IsNotSame(
                    auditUser.EntraUserId.ToString(),
                    pdsData.UpdatedBy,
                    nameof(PdsData.UpdatedBy)),
                 Parameter: nameof(PdsData.UpdatedBy))
            );
        }

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