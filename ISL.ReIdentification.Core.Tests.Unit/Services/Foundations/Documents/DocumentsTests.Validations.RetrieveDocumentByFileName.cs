﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Theory]
        [MemberData(nameof(InvalidArgumentsStreamHasLength))]
        public async Task ShouldThrowValidationExceptionOnRetrieveDocumentByFileNameIfArgumentsInvalidAndLogItAsync(
            Stream invalidStream, string invalidString)
        {
            // given
            Stream invalidInput = invalidStream;
            string invalidFileName = invalidString;
            string invalidContainer = invalidString;

            var invalidDocumentException = new InvalidDocumentException(
                message: "Invalid document. Please correct the errors and try again.");

            invalidDocumentException.AddData(
                key: "output",
                values: "Stream is invalid");

            invalidDocumentException.AddData(
                key: "fileName",
                values: "Text is invalid");

            invalidDocumentException.AddData(
                key: "container",
                values: "Text is invalid");

            var expectedDocumentValidationException = new DocumentValidationException(
                message: "Document validation error occurred, please fix errors and try again.",
                innerException: invalidDocumentException);

            // when
            ValueTask retrieveDocumentTask =
                this.documentService.RetrieveDocumentByFileNameAsync(invalidStream, invalidFileName, invalidContainer);

            DocumentValidationException actualDocumentValidationException =
                await Assert.ThrowsAsync<DocumentValidationException>(retrieveDocumentTask.AsTask);

            // then
            actualDocumentValidationException.Should().BeEquivalentTo(expectedDocumentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDocumentValidationException))),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
