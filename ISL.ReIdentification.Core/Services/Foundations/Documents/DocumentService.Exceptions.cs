// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Foundations.Documents
{
    public partial class DocumentService : IDocumentService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (InvalidDocumentException invalidDocumentException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidDocumentException);
            }
        }

        private async ValueTask<DocumentValidationException> CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var documentValidationException =
                new DocumentValidationException(
                    message: "Document validation error occurred, please fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(documentValidationException);

            return documentValidationException;
        }
    }
}
