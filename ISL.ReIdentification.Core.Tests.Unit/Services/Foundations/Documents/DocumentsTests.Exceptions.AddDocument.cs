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
        [Fact]
        public async Task ShouldThrowServicenExceptionAddDocumentAndLogItAsync()
        {
            // given
            Stream someStream = new HasLengthStream();
            string someFileName = GetRandomString();
            string someContainer = GetRandomString();
            Exception someException = new Exception();

            var failedServiceDocumentException = new FailedServiceDocumentException(
                message: "Failed service document error occurred, contact support.",
                innerException: someException);

            var expectedDocumentServiceException = new DocumentServiceException(
                message: "Document service error occurred, contact support.",
                innerException: failedServiceDocumentException);

            // when
            ValueTask addDocumentTask =
                this.documentService.AddDocumentAsync(someStream, someFileName, someContainer);

            DocumentServiceException actualDocumentServiceException =
                await Assert.ThrowsAsync<DocumentServiceException>(addDocumentTask.AsTask);

            // then
            actualDocumentServiceException.Should().BeEquivalentTo(expectedDocumentServiceException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDocumentServiceException))),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
