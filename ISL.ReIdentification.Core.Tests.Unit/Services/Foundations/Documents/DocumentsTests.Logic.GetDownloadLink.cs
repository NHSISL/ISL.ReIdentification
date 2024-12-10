// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Fact]
        public async Task ShouldGetDownloadLinkAsync()
        {
            // given
            string randomFileName = GetRandomString();
            string randomContainer = GetRandomString();
            DateTimeOffset futureDateTimeOffset = GetRandomFutureDateTimeOffset();

            // when
            await this.documentService.GetDownloadLinkAsync(randomFileName, randomContainer, futureDateTimeOffset);

            // then
            this.blobStorageBrokerMock.Verify(broker =>
                broker.GetDownloadLinkAsync(randomFileName, randomContainer, futureDateTimeOffset),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}