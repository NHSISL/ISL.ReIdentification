// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions;

namespace ISL.ReIdentification.Core.Services.Foundations.Documents
{
    public partial class DocumentService : IDocumentService
    {
        private static void ValidateOnAddDocument(Stream input, string fileName, string container)
        {
            Validate(
                (Rule: IsInvalid(fileName), Parameter: nameof(fileName)),
                (Rule: IsInvalid(container), Parameter: nameof(container)),
                (Rule: IsInvalidInputStream(input), Parameter: nameof(input)));
        }

        private static dynamic IsInvalid(string value) => new
        {
            Condition = string.IsNullOrWhiteSpace(value),
            Message = "Text is invalid"
        };

        private static dynamic IsInvalidInputStream(Stream inputStream) => new
        {
            Condition = inputStream is null || inputStream.Length == 0,
            Message = "Stream is invalid"
        };


        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidDocumentException = new InvalidDocumentException(
                message: "Invalid document. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDocumentException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDocumentException.ThrowIfContainsErrors();
        }
    }
}
