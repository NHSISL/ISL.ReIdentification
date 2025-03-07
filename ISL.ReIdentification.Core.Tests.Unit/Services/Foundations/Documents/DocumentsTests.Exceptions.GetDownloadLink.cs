// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnGetDownloadAsync(
            Xeption dependencyValidationException)
        {
            // given
            string someContainer = GetRandomString();
            string someFileName = GetRandomString();
            DateTimeOffset someDate = GetRandomFutureDateTimeOffset();

            var expectedDependencyValidationException = new DocumentDependencyValidationException(
                message: "Document dependency validation error occurred, please fix errors and try again.",
                innerException: dependencyValidationException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.GetDownloadLinkAsync(someContainer, someFileName, someDate))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<string> getDownloadLinkTask =
                this.documentService.GetDownloadLinkAsync(someContainer, someFileName, someDate);

            DocumentDependencyValidationException actualDependencyValidationException =
                await Assert.ThrowsAsync<DocumentDependencyValidationException>(getDownloadLinkTask.AsTask);

            // then
            actualDependencyValidationException.Should().BeEquivalentTo(expectedDependencyValidationException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.GetDownloadLinkAsync(someContainer, someFileName, someDate),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedDependencyValidationException))),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDocumentDependencyExceptionOnGetDownloadAsync(Xeption dependencyException)
        {
            // given
            string someContainer = GetRandomString();
            string someFileName = GetRandomString();
            DateTimeOffset someDate = GetRandomFutureDateTimeOffset();

            var expectedDocumentDependencyException = new DocumentDependencyException(
                message: "Document dependency error occurred, please fix errors and try again.",
                innerException: dependencyException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.GetDownloadLinkAsync(someContainer, someFileName, someDate))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<string> getDownloadLinkTask =
                this.documentService.GetDownloadLinkAsync(someContainer, someFileName, someDate);

            DocumentDependencyException actualDocumentDependencyException =
                await Assert.ThrowsAsync<DocumentDependencyException>(getDownloadLinkTask.AsTask);

            // then
            actualDocumentDependencyException.Should().BeEquivalentTo(expectedDocumentDependencyException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.GetDownloadLinkAsync(someContainer, someFileName, someDate),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDocumentDependencyException))),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnGetDownloadLinkAsync()
        {
            // given
            string someContainer = GetRandomString();
            string someFileName = GetRandomString();
            DateTimeOffset someDate = GetRandomFutureDateTimeOffset();
            Exception someException = new Exception();

            var failedServiceDocumentException = new FailedServiceDocumentException(
                message: "Failed service document error occurred, contact support.",
                innerException: someException);

            var expectedDocumentServiceException = new DocumentServiceException(
                message: "Document service error occurred, contact support.",
                innerException: failedServiceDocumentException);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.GetDownloadLinkAsync(someContainer, someFileName, someDate))
                    .ThrowsAsync(someException);

            // when
            ValueTask<string> getDownloadLinkTask =
                this.documentService.GetDownloadLinkAsync(someContainer, someFileName, someDate);

            DocumentServiceException actualDocumentServiceException =
                await Assert.ThrowsAsync<DocumentServiceException>(getDownloadLinkTask.AsTask);

            // then
            actualDocumentServiceException.Should().BeEquivalentTo(expectedDocumentServiceException);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.GetDownloadLinkAsync(someContainer, someFileName, someDate),
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
