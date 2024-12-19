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
        public async Task ShouldRetrieveListOfAllAccessPoliciesAsync()
        {
            // given
            string randomContainer = GetRandomString();
            List<string> randomAccessPolicies = GetRandomStringList();
            List<string> outputAccessPolicies = randomAccessPolicies;
            List<string> expectedAccessPolicies = outputAccessPolicies;

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RetrieveListOfAllAccessPoliciesAsync(randomContainer))
                    .ReturnsAsync(outputAccessPolicies);

            // when
            List<string> actualAccessPolicies = 
                await this.documentService.RetrieveListOfAllAccessPoliciesAsync(randomContainer);

            // then
            actualAccessPolicies.Should().BeEquivalentTo(expectedAccessPolicies);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RetrieveListOfAllAccessPoliciesAsync(randomContainer),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
