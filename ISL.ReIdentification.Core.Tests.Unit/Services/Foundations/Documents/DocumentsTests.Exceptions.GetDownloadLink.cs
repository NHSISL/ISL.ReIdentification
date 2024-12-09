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
        public async Task ShouldThrowDependencyValidationExceptionOnGetDownloadAsync(Xeption dependencyValidationException)
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
    }
}
