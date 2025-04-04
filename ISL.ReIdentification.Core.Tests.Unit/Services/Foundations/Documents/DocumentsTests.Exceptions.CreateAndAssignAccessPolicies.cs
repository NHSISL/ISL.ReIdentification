﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.Storages.Abstractions.Models;
using ISL.ReIdentification.Core.Models.Foundations.Documents;
using ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnCreateAndAssignAccessPoliciesAsync(
            Xeption dependencyValidationException)
        {
            // given
            string someContainer = GetRandomString();
            List<AccessPolicy> someAccessPolicies = GetAccessPolicies();

            var expectedDependencyValidationException = new DocumentDependencyValidationException(
                message: "Document dependency validation error occurred, please fix errors and try again.",
                innerException: dependencyValidationException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.CreateAndAssignAccessPoliciesAsync(someContainer, It.IsAny<List<Policy>>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask createAndAssignAccessPoliciesTask =
                this.documentService.CreateAndAssignAccessPoliciesAsync(someContainer, someAccessPolicies);

            DocumentDependencyValidationException actualDocumentServiceException =
                await Assert.ThrowsAsync<DocumentDependencyValidationException>(
                    testCode: createAndAssignAccessPoliciesTask.AsTask);

            // then
            actualDocumentServiceException.Should().BeEquivalentTo(expectedDependencyValidationException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.CreateAndAssignAccessPoliciesAsync(someContainer, It.IsAny<List<Policy>>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedDependencyValidationException))),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDocumentDependencyExceptionOnCreateAndAssignAccessPoliciesAsync(
            Xeption dependencyException)
        {
            // given
            string someContainer = GetRandomString();
            List<AccessPolicy> someAccessPolicies = GetAccessPolicies();

            var expectedDocumentDependencyException = new DocumentDependencyException(
                message: "Document dependency error occurred, please fix errors and try again.",
                innerException: dependencyException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.CreateAndAssignAccessPoliciesAsync(someContainer, It.IsAny<List<Policy>>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask createAndAssignAccessPoliciesTask =
                this.documentService.CreateAndAssignAccessPoliciesAsync(someContainer, someAccessPolicies);

            DocumentDependencyException actualDocumentDependencyException =
                await Assert.ThrowsAsync<DocumentDependencyException>(
                    testCode: createAndAssignAccessPoliciesTask.AsTask);

            // then
            actualDocumentDependencyException.Should().BeEquivalentTo(expectedDocumentDependencyException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.CreateAndAssignAccessPoliciesAsync(someContainer, It.IsAny<List<Policy>>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedDocumentDependencyException))),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnCreateAndAssignAccessPoliciesAsync()
        {
            // given
            string someContainer = GetRandomString();
            List<AccessPolicy> someAccessPolicies = GetAccessPolicies();
            Exception someException = new Exception();

            var failedServiceDocumentException = new FailedServiceDocumentException(
                message: "Failed service document error occurred, contact support.",
                innerException: someException);

            var expectedDocumentServiceException = new DocumentServiceException(
                message: "Document service error occurred, contact support.",
                innerException: failedServiceDocumentException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.CreateAndAssignAccessPoliciesAsync(someContainer, It.IsAny<List<Policy>>()))
                    .ThrowsAsync(someException);

            // when
            ValueTask createAndAssignAccessPoliciesTask =
                this.documentService.CreateAndAssignAccessPoliciesAsync(someContainer, someAccessPolicies);

            DocumentServiceException actualDocumentServiceException =
                await Assert.ThrowsAsync<DocumentServiceException>(
                    testCode: createAndAssignAccessPoliciesTask.AsTask);

            // then
            actualDocumentServiceException.Should().BeEquivalentTo(expectedDocumentServiceException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.CreateAndAssignAccessPoliciesAsync(someContainer, It.IsAny<List<Policy>>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedDocumentServiceException))),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
