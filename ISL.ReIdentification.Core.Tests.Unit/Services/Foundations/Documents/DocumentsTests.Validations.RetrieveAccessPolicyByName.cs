// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.Storages.Abstractions.Models;
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
        public async Task ShouldThrowValidationExceptionOnRetrieveAccessPolicyAsync(
            string invalidString)
        {
            // given
            string invalidContainer = invalidString;
            string invalidPolicyName = invalidString;

            var invalidDocumentException = new InvalidArgumentDocumentException(
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
            ValueTask<Policy> retrievedPolicyTask =
                this.documentService.RetrieveAccessPolicyByNameAsync(invalidContainer, invalidPolicyName);

            DocumentValidationException actualDocumentValidationException =
                await Assert.ThrowsAsync<DocumentValidationException>(retrievedPolicyTask.AsTask);

            // then
            actualDocumentValidationException.Should().BeEquivalentTo(expectedDocumentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDocumentValidationException))),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByNameIfNotExistsAndLogItAsync()
        {
            string someContainer = GetRandomString();
            string somePolicyName = GetRandomString();
            List<string> somePolicies = GetRandomStringList();

            var accessPolicyNotFoundStorageException =
                new AccessPolicyNotFoundDocumentException(
                    message: "Access policy with the provided name was not found on this container.");

            var expectedDocumentValidationException = new DocumentValidationException(
                message: "Document validation error occurred, please fix errors and try again.",
                innerException: accessPolicyNotFoundStorageException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RetrieveAllAccessPoliciesFromContainerAsync(someContainer))
                    .ReturnsAsync(somePolicies);

            // when
            ValueTask<Policy> retrieveAccessPolicyTask =
                this.documentService.RetrieveAccessPolicyByNameAsync(someContainer, somePolicyName);

            DocumentValidationException actualDocumentValidationException =
                await Assert.ThrowsAsync<DocumentValidationException>(
                    testCode: retrieveAccessPolicyTask.AsTask);

            // then
            actualDocumentValidationException.Should().BeEquivalentTo(expectedDocumentValidationException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RetrieveAllAccessPoliciesFromContainerAsync(someContainer),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDocumentValidationException))),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
