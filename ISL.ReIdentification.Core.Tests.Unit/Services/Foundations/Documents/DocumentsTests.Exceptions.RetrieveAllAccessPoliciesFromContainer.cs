// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
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
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveAllAccessPoliciesFromContainerAsync(Xeption dependencyValidationException)
        {
            // given
            string someContainer = GetRandomString();

            var expectedDependencyValidationException = new DocumentDependencyValidationException(
                message: "Document dependency validation error occurred, please fix errors and try again.",
                innerException: dependencyValidationException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RetrieveAllAccessPoliciesFromContainerAsync(someContainer))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<List<string>> retrieveAccessPolicyTask =
                this.documentService.RetrieveAllAccessPoliciesFromContainerAsync(someContainer);

            DocumentDependencyValidationException actualDocumentServiceException =
                await Assert.ThrowsAsync<DocumentDependencyValidationException>(retrieveAccessPolicyTask.AsTask);

            // then
            actualDocumentServiceException.Should().BeEquivalentTo(expectedDependencyValidationException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RetrieveAllAccessPoliciesFromContainerAsync(someContainer),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedDependencyValidationException))),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDocumentDependencyExceptionOnRetrieveAllAccessPoliciesFromContainerAsync(Xeption dependencyException)
        {
            // given
            string someContainer = GetRandomString();

            var expectedDocumentDependencyException = new DocumentDependencyException(
                message: "Document dependency error occurred, please fix errors and try again.",
                innerException: dependencyException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RetrieveAllAccessPoliciesFromContainerAsync(someContainer))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<List<string>> retrieveAccessPolicyTask =
                this.documentService.RetrieveAllAccessPoliciesFromContainerAsync(someContainer);

            DocumentDependencyException actualDocumentDependencyException =
                await Assert.ThrowsAsync<DocumentDependencyException>(retrieveAccessPolicyTask.AsTask);

            // then
            actualDocumentDependencyException.Should().BeEquivalentTo(expectedDocumentDependencyException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RetrieveAllAccessPoliciesFromContainerAsync(someContainer),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDocumentDependencyException))),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllAccessPoliciesFromContainerAsync()
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
                broker.RetrieveAllAccessPoliciesFromContainerAsync(someContainer))
                    .ThrowsAsync(someException);

            // when
            ValueTask<List<string>> retrieveAccessPolicyTask =
                this.documentService.RetrieveAllAccessPoliciesFromContainerAsync(someContainer);

            DocumentServiceException actualDocumentServiceException =
                await Assert.ThrowsAsync<DocumentServiceException>(retrieveAccessPolicyTask.AsTask);

            // then
            actualDocumentServiceException.Should().BeEquivalentTo(expectedDocumentServiceException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RetrieveAllAccessPoliciesFromContainerAsync(someContainer),
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
