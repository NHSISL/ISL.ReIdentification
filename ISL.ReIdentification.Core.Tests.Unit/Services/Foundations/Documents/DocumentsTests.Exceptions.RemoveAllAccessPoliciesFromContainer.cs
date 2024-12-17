// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.Storages.Abstractions.Models.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveAllAccessPoliciesAsync()
        {
            // given
            string someContainer = GetRandomString();

            var storageProviderValidationException = new StorageProviderValidationException(
                message: "Storage provider validation errors occurred, please try again.",
                innerException: new Xeption());

            var expectedDocumentServiceException = new DocumentDependencyValidationException(
                message: "Document dependency validation error occurred, please fix errors and try again.",
                innerException: storageProviderValidationException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RemoveAllAccessPoliciesAsync(someContainer))
                    .ThrowsAsync(storageProviderValidationException);

            // when
            ValueTask removeAccessPolicyTask =
                this.documentService.RemoveAllAccessPoliciesAsync(someContainer);

            DocumentDependencyValidationException actualDocumentServiceException =
                await Assert.ThrowsAsync<DocumentDependencyValidationException>(removeAccessPolicyTask.AsTask);

            // then
            actualDocumentServiceException.Should().BeEquivalentTo(expectedDocumentServiceException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RemoveAllAccessPoliciesAsync(someContainer),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDocumentServiceException))),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDocumentDependencyExceptionOnRemoveAllAccessPoliciesAsync(Xeption dependencyException)
        {
            // given
            string someContainer = GetRandomString();

            var expectedDocumentDependencyException = new DocumentDependencyException(
                message: "Document dependency error occurred, please fix errors and try again.",
                innerException: dependencyException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RemoveAllAccessPoliciesAsync(someContainer))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask removeAccessPolicyTask =
                this.documentService.RemoveAllAccessPoliciesAsync(someContainer);

            DocumentDependencyException actualDocumentDependencyException =
                await Assert.ThrowsAsync<DocumentDependencyException>(removeAccessPolicyTask.AsTask);

            // then
            actualDocumentDependencyException.Should().BeEquivalentTo(expectedDocumentDependencyException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RemoveAllAccessPoliciesAsync(someContainer),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDocumentDependencyException))),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveAllAccessPoliciesAsync()
        {
            // given
            string someContainer = GetRandomString();
            Exception someException = new Exception();

            var failedServiceDocumentException = new FailedServiceDocumentException(
                message: "Failed service document error occurred, contact support.",
                innerException: someException);

            var expectedDocumentServiceException = new DocumentServiceException(
                message: "Document service error occurred, contact support.",
                innerException: failedServiceDocumentException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RemoveAllAccessPoliciesAsync(someContainer))
                    .ThrowsAsync(someException);

            // when
            ValueTask removeAccessPolicyTask =
                this.documentService.RemoveAllAccessPoliciesAsync(someContainer);

            DocumentServiceException actualDocumentServiceException =
                await Assert.ThrowsAsync<DocumentServiceException>(removeAccessPolicyTask.AsTask);

            // then
            actualDocumentServiceException.Should().BeEquivalentTo(expectedDocumentServiceException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RemoveAllAccessPoliciesAsync(someContainer),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDocumentServiceException))),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
