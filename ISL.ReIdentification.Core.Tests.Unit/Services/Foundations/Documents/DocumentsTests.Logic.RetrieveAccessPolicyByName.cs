﻿// ---------------------------------------------------------
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
        public async Task ShouldRetrieveAccessPolicyByNameAsync()
        {
            // given
            string randomContainer = GetRandomString();
            string inputContainer = randomContainer;
            string randomPolicyName = GetRandomString();
            dynamic randomPolicyProperties = CreateRandomPolicyProperties();
            string inputPolicyName = randomPolicyProperties.PolicyName;

            Policy outputPolicy = new Policy
            {
                PolicyName = randomPolicyProperties.PolicyName,
                Permissions = randomPolicyProperties.Permissions,
            };

            AccessPolicy expectedAccessPolicy = new AccessPolicy
            {
                PolicyName = randomPolicyProperties.PolicyName,
                Permissions = randomPolicyProperties.Permissions,
            };

            List<string> outputPolicyNames = new List<string>
            {
                inputPolicyName,
            };

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RetrieveListOfAllAccessPoliciesAsync(inputContainer))
                    .ReturnsAsync(outputPolicyNames);

            this.blobStorageBrokerMock.Setup(broker =>
                broker.RetrieveAccessPolicyByNameAsync(
                    inputContainer,
                    inputPolicyName))
                        .ReturnsAsync(outputPolicy);

            // when
            AccessPolicy actualAccessPolicy =
                await this.documentService.RetrieveAccessPolicyByNameAsync(inputContainer, inputPolicyName);

            // then
            actualAccessPolicy.Should().BeEquivalentTo(expectedAccessPolicy);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RetrieveListOfAllAccessPoliciesAsync(inputContainer),
                    Times.Once);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.RetrieveAccessPolicyByNameAsync(
                    inputContainer,
                    inputPolicyName),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
