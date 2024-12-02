// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Fact]
        public async Task ShouldRetrieveAllAccessPoliciesFromContainerAsync()
        {
            // given
            string randomContainer = GetRandomString();
            List<string> randomAccessPolicies = GetRandomStringList();
            List<string> outputAccessPolicies = randomAccessPolicies;
            List<string> expectedAccessPolicies = outputAccessPolicies;

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RetrieveAllAccessPoliciesFromContainerAsync(randomContainer))
                    .ReturnsAsync(outputAccessPolicies);

            // when
            List<string> actualAccessPolicies = 
                await this.documentService.RetrieveAllAccessPoliciesFromContainerAsync(randomContainer);

            // then
            actualAccessPolicies.Should().BeEquivalentTo(expectedAccessPolicies);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RetrieveAllAccessPoliciesFromContainerAsync(randomContainer),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
