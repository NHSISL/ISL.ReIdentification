// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.Storages.Abstractions.Models;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Fact]
        public async Task ShouldRetrieveAllAccessPoliciesAsync()
        {
            // given
            string randomContainer = GetRandomString();
            List<Policy> randomAccessPolicies = GetPolicies();
            List<Policy> outputAccessPolicies = randomAccessPolicies;
            List<Policy> expectedAccessPolicies = outputAccessPolicies;

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RetrieveAllAccessPoliciesAsync(randomContainer))
                    .ReturnsAsync(outputAccessPolicies);

            // when
            List<Policy> actualAccessPolicies = 
                await this.documentService.RetrieveAllAccessPoliciesAsync(randomContainer);

            // then
            actualAccessPolicies.Should().BeEquivalentTo(expectedAccessPolicies);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RetrieveAllAccessPoliciesAsync(randomContainer),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
