// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Fact]
        public async Task ShouldRemoveDocumentByFileNameAsync()
        {
            // given
            string randomFileName = GetRandomString();
            string randomContainer = GetRandomString();

            // when
            await this.documentService.RemoveDocumentByFileNameAsync(randomFileName, randomContainer);

            // then
            this.blobStorageBrokerMock.Verify(broker =>
                broker.DeleteFileAsync(randomFileName, randomContainer),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
