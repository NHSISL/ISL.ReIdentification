// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Fact]
        public async Task ShouldAddFolderAsync()
        {
            // given
            string randomFolderName = GetRandomString();
            string randomContainer = GetRandomString();

            // when
            await this.documentService.AddFolderAsync(randomContainer, randomFolderName);

            // then
            this.blobStorageBrokerMock.Verify(broker =>
                broker.CreateFolderInContainerAsync(randomContainer, randomFolderName),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}