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
        public async Task ShouldRetrieveAllContainersAsync()
        {
            // given
            List<string> randomContainers = GetRandomStringList();
            List<string> outputContainers = randomContainers;
            List<string> expectedContainers = outputContainers;

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RetrieveAllContainersAsync())
                    .ReturnsAsync(outputContainers);

            // when
            List<string> actualContainers =
                await this.documentService.RetrieveAllContainersAsync();

            // then
            actualContainers.Should().BeEquivalentTo(expectedContainers);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RetrieveAllContainersAsync(),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
