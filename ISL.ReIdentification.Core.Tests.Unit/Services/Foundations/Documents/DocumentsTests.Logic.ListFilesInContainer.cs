// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Fact]
        public async Task ShouldListFilesInContainerAsync()
        {
            // given
            string randomContainer = GetRandomString();
            List<string> randomFileList = GetRandomStringList();
            List<string> outputFileList = randomFileList;
            List<string> expectedFileList = outputFileList;

            this.blobStorageBrokerMock.Setup(broker =>
                broker.ListFilesInContainerAsync(randomContainer))
                    .ReturnsAsync(outputFileList);

            // when
            List<string> actualFileList =
                await this.documentService.ListFilesInContainerAsync(randomContainer);

            // then
            actualFileList.Should().BeEquivalentTo(expectedFileList);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.ListFilesInContainerAsync(randomContainer),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}