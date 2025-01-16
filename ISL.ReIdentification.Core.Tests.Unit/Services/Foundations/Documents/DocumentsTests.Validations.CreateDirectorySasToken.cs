// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnCreateSasTokenAsync(
            string invalidString)
        {
            // given
            string invalidContainer = invalidString;
            string invalidpath = invalidString;
            string invalidAccessPolicyIdentifier = invalidString;
            DateTimeOffset invalidDateTimeOffset = GetRandomFutureDateTimeOffset();

            var invalidDocumentException = new InvalidDocumentException(
                message: "Invalid document. Please correct the errors and try again.");

            invalidDocumentException.AddData(
                key: "path",
                values: "Text is invalid");

            invalidDocumentException.AddData(
                key: "container",
                values: "Text is invalid");

            invalidDocumentException.AddData(
                key: "accessPolicyIdentifier",
                values: "Text is invalid");

            var expectedDocumentValidationException = new DocumentValidationException(
                message: "Document validation error occurred, please fix errors and try again.",
                innerException: invalidDocumentException);

            // when
            ValueTask<string> createDirectorySasTokenTask =
                this.documentService.CreateSasTokenAsync(
                    invalidContainer, 
                    invalidpath, 
                    invalidAccessPolicyIdentifier, 
                    invalidDateTimeOffset);

            DocumentValidationException actualDocumentValidationException =
                await Assert.ThrowsAsync<DocumentValidationException>(createDirectorySasTokenTask.AsTask);

            // then
            actualDocumentValidationException.Should().BeEquivalentTo(expectedDocumentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedDocumentValidationException))),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}