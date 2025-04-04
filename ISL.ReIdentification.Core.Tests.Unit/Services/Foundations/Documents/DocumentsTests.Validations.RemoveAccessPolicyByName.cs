﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnRemoveAccessPolicyByNameAsync(
            string invalidString)
        {
            // given
            string invalidContainer = invalidString;
            string invalidPolicyName = invalidString;

            var invalidDocumentException = new InvalidDocumentException(
                message: "Invalid document. Please correct the errors and try again.");

            invalidDocumentException.AddData(
                key: "Container",
                values: "Text is invalid");

            invalidDocumentException.AddData(
                key: "PolicyName",
                values: "Text is invalid");

            var expectedDocumentValidationException = new DocumentValidationException(
                message: "Document validation error occurred, please fix errors and try again.",
                innerException: invalidDocumentException);

            // when
            ValueTask removedPoliciesTask =
                this.documentService.RemoveAccessPolicyByNameAsync(
                    invalidContainer,
                    invalidPolicyName);

            DocumentValidationException actualDocumentValidationException =
                await Assert.ThrowsAsync<DocumentValidationException>(removedPoliciesTask.AsTask);

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
