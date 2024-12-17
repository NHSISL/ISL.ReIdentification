// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.Storages.Abstractions.Models;
using ISL.ReIdentification.Core.Models.Foundations.Documents;
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
            List<dynamic> randomPolicyPropertiesList = CreateRandomPolicyPropertiesList();
            List<Policy> outputPolicies = new List<Policy>();
            List<AccessPolicy> expectedAccessPolicies = new List<AccessPolicy>();

            foreach (dynamic policyProperties in randomPolicyPropertiesList)
            {
                Policy policy = new Policy
                {
                    PolicyName = policyProperties.PolicyName,
                    Permissions = policyProperties.Permissions,
                };

                AccessPolicy accessPolicy = new AccessPolicy
                {
                    PolicyName = policyProperties.PolicyName,
                    Permissions = policyProperties.Permissions,
                };

                outputPolicies.Add(policy);
                expectedAccessPolicies.Add(accessPolicy);
            }

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RetrieveAllAccessPoliciesAsync(randomContainer))
                    .ReturnsAsync(outputPolicies);

            // when
            List<AccessPolicy> actualAccessPolicies =
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
