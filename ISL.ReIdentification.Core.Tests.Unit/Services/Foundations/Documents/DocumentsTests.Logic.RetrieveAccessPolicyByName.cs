// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.Storages.Abstractions.Models;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Fact]
        public async Task ShouldRetrieveAccessPolicyByNameAsync()
        {
            // given
            string randomContainer = GetRandomString();
            string randomPolicyName = GetRandomString();
            Policy randomAccessPolicy = GetPolicy();
            Policy outputAccessPolicy = randomAccessPolicy;
            Policy expectedAccessPolicies = outputAccessPolicy;

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RetrieveAccessPolicyByNameAsync(
                    randomContainer, 
                    randomPolicyName))
                .ReturnsAsync(outputAccessPolicy);

            // when
            Policy actualAccessPolicy = 
                await this.documentService.RetrieveAccessPolicyByNameAsync(randomContainer, randomPolicyName);

            // then
            actualAccessPolicy.Should().BeEquivalentTo(expectedAccessPolicies);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RetrieveAccessPolicyByNameAsync(
                    randomContainer, 
                    randomPolicyName),
                Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
