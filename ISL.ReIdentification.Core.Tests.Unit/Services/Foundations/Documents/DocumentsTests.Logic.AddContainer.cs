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
        public async Task ShouldAddContainerAsync()
        {
            // given
            string randomContainer = GetRandomString();

            // when
            await this.documentService.AddContainerAsync(randomContainer);

            // then
            this.blobStorageBrokerMock.Verify(broker =>
                broker.CreateContainerAsync(randomContainer),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}