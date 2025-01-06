// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.Providers.Storages.Abstractions.Models;
using ISL.ReIdentification.Core.Models.Foundations.Documents;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Fact]
        public async Task ShouldCreateAndAssignAccessPoliciesAsync()
        {
            // given
            string randomContainer = GetRandomString();
            string inputContainer = randomContainer;
            dynamic randomPolicyProperties = CreateRandomPolicyProperties();

            List<AccessPolicy> inputAccessPolicies = new List<AccessPolicy>
            {
                new AccessPolicy
                {
                    PolicyName = randomPolicyProperties.PolicyName,
                    Permissions = randomPolicyProperties.Permissions,
                }
            };

            List<Policy> expectedInputPolicies = new List<Policy>
            {
                new Policy
                {
                    PolicyName = randomPolicyProperties.PolicyName,
                    Permissions = randomPolicyProperties.Permissions,
                }
            };

            // when
            await this.documentService
                .CreateAndAssignAccessPoliciesAsync(inputContainer, inputAccessPolicies);

            // then
            this.blobStorageBrokerMock.Verify(broker =>
                broker.CreateAndAssignAccessPoliciesAsync(
                    inputContainer,
                    It.Is(SamePolicyListAs(expectedInputPolicies))),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
