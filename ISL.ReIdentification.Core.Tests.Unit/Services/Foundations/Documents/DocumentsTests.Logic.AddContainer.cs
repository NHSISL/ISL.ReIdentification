// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
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
            string inputContainer = randomContainer;
            List<string> randomContainers = GetRandomStringList();

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RetrieveAllContainersAsync())
                    .ReturnsAsync(randomContainers);

            // when
            await this.documentService.AddContainerAsync(randomContainer);

            // then
            this.blobStorageBrokerMock.Verify(broker =>
                broker.RetrieveAllContainersAsync(),
                    Times.Once);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.CreateContainerAsync(randomContainer),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnEarlyOnAddContainerIfAlreadyExistsAsync()
        {
            // given
            string randomContainer = GetRandomString();
            string inputContainer = randomContainer;
            List<string> returnedContainers = new List<string> { inputContainer };

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RetrieveAllContainersAsync())
                    .ReturnsAsync(returnedContainers);

            // when
            await this.documentService.AddContainerAsync(inputContainer);

            // then
            this.blobStorageBrokerMock.Verify(broker =>
                broker.RetrieveAllContainersAsync(),
                    Times.Once);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.CreateContainerAsync(randomContainer),
                    Times.Never);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}