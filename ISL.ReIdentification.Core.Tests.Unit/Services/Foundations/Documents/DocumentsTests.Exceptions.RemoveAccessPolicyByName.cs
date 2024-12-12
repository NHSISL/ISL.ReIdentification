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
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveAccessPolicyByNameAsync(
            Xeption dependencyValidationException)
        {
            // given
            string someContainer = GetRandomString();
            string somePolicyName = GetRandomString();
                
            var expectedDocumentServiceException = new DocumentDependencyValidationException(
                message: "Document dependency validation error occurred, please fix errors and try again.",
                innerException: dependencyValidationException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RemoveAccessPolicyByNameAsync(someContainer, somePolicyName))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask removeAccessPolicyByNameTask =
                this.documentService.RemoveAccessPolicyByNameAsync(someContainer, somePolicyName);

            DocumentDependencyValidationException actualDocumentServiceException =
                await Assert.ThrowsAsync<DocumentDependencyValidationException>(removeAccessPolicyByNameTask.AsTask);

            // then
            actualDocumentServiceException.Should().BeEquivalentTo(expectedDocumentServiceException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RemoveAccessPolicyByNameAsync(
                    someContainer,
                    somePolicyName),
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
        public async Task ShouldThrowDocumentDependencyExceptionOnRemoveAccessPolicyByNameAsync(
            Xeption dependencyException)
        {
            // given
            string someContainer = GetRandomString();
            string somePolicyName = GetRandomString();
            
            var expectedDocumentDependencyException = new DocumentDependencyException(
                message: "Document dependency error occurred, please fix errors and try again.",
                innerException: dependencyException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RemoveAccessPolicyByNameAsync(
                    someContainer, 
                    somePolicyName))
                .ThrowsAsync(dependencyException);

            // when
            ValueTask removeAccessPolicyByNameTask =
                this.documentService.RemoveAccessPolicyByNameAsync(
                    someContainer,
                    somePolicyName);

            DocumentDependencyException actualDocumentDependencyException =
                await Assert.ThrowsAsync<DocumentDependencyException>(removeAccessPolicyByNameTask.AsTask);

            // then
            actualDocumentDependencyException.Should().BeEquivalentTo(expectedDocumentDependencyException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RemoveAccessPolicyByNameAsync(
                    someContainer, 
                    somePolicyName),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDocumentDependencyException))),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
