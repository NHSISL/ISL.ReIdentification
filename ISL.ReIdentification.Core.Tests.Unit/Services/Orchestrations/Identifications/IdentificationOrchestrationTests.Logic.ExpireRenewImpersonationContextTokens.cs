// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Foundations.Documents;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldExpireRenewImpersonationContextTokensAsync(bool containerExists)
        {
            // given
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.CsvIdentificationRequest = null;
            randomAccessRequest.IdentificationRequest = null;
            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            string inputContainer = inputAccessRequest.ImpersonationContext.Id.ToString();
            List<string> randomAccessPolicies = GetRandomStringList();
            List<string> outputAccessPolicies = randomAccessPolicies;
            List<string> randomContainers = GetRandomStringList();
            List<string> outputContainers = randomContainers;

            if (containerExists)
            {
                outputContainers.Add(inputAccessRequest.ImpersonationContext.Id.ToString());
            }

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string timestamp = randomDateTimeOffset.ToString("yyyyMMddHHmmss");
            string inputInboxPolicyname = inputContainer + "-InboxPolicy-" + timestamp;
            string inputOutboxPolicyname = inputContainer + "-OutboxPolicy-" + timestamp;
            string inputErrorsPolicyname = inputContainer + "-ErrorsPolicy-" + timestamp;

            List<AccessPolicy> inputAccessPolicies = GetAccessPolicies(
                inputInboxPolicyname,
                inputOutboxPolicyname,
                inputErrorsPolicyname);

            ProjectStorageConfiguration randomProjectStorageConfiguration =
                CreateRandomProjectStorageConfiguration();


            DateTimeOffset inputExpiresOn = randomDateTimeOffset
                .AddMinutes(randomProjectStorageConfiguration.TokenLifetimeMinutes);

            string randomInboxSasToken = GetRandomString();
            string outputInboxSasToken = randomInboxSasToken;
            string randomOutboxSasToken = GetRandomString();
            string outputOutboxSasToken = randomOutboxSasToken;
            string randomErrorsSasToken = GetRandomString();
            string outputErrorsSasToken = randomErrorsSasToken;
            outputAccessRequest.ImpersonationContext.InboxSasToken = outputInboxSasToken;
            outputAccessRequest.ImpersonationContext.OutboxSasToken = outputOutboxSasToken;
            outputAccessRequest.ImpersonationContext.ErrorsSasToken = outputErrorsSasToken;
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.documentServiceMock.Setup(service =>
                service.RetrieveAllContainersAsync())
                    .ReturnsAsync(outputContainers);

            this.documentServiceMock.Setup(service =>
                service.RetrieveListOfAllAccessPoliciesAsync(inputContainer))
                    .ReturnsAsync(outputAccessPolicies);

            this.documentServiceMock.Setup(service =>
                service.CreateSasTokenAsync(
                    inputContainer,
                    randomProjectStorageConfiguration.PickupFolder,
                    inputInboxPolicyname,
                    inputExpiresOn))
                        .ReturnsAsync(outputInboxSasToken);

            this.documentServiceMock.Setup(service =>
                service.CreateSasTokenAsync(
                    inputContainer,
                    randomProjectStorageConfiguration.LandingFolder,
                    inputOutboxPolicyname,
                    inputExpiresOn))
                        .ReturnsAsync(outputOutboxSasToken);

            this.documentServiceMock.Setup(service =>
                service.CreateSasTokenAsync(
                    inputContainer,
                    randomProjectStorageConfiguration.ErrorFolder,
                    inputErrorsPolicyname,
                    inputExpiresOn))
                        .ReturnsAsync(outputErrorsSasToken);

            var identificationOrchestrationServiceMock =
                new Mock<IdentificationOrchestrationService>(
                    this.reIdentificationServiceMock.Object,
                    this.accessAuditServiceMock.Object,
                    this.documentServiceMock.Object,
                    this.loggingBrokerMock.Object,
                    this.dateTimeBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    randomProjectStorageConfiguration)
                { CallBase = true };

            IdentificationOrchestrationService service = identificationOrchestrationServiceMock.Object;

            // when
            AccessRequest actualAccessRequest = await service
                .ExpireRenewImpersonationContextTokensAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.documentServiceMock.Verify(service =>
                service.RetrieveAllContainersAsync(),
                    Times.Once);

            this.documentServiceMock.Verify(service =>
                service.RetrieveListOfAllAccessPoliciesAsync(inputContainer),
                    Times.Once);

            if (!containerExists)
            {
                this.documentServiceMock.Verify(service =>
                    service.AddContainerAsync(inputContainer),
                        Times.Once);

                this.documentServiceMock.Verify(service =>
                    service.AddFolderAsync(
                        inputContainer,
                        randomProjectStorageConfiguration.PickupFolder),
                            Times.Once);

                this.documentServiceMock.Verify(service =>
                    service.AddFolderAsync(
                        inputContainer,
                        randomProjectStorageConfiguration.LandingFolder),
                            Times.Once);

                this.documentServiceMock.Verify(service =>
                    service.AddFolderAsync(
                        inputContainer,
                        randomProjectStorageConfiguration.ErrorFolder),
                            Times.Once);
            }

            this.documentServiceMock.Verify(service =>
                service.RemoveAllAccessPoliciesAsync(inputContainer),
                    Times.Once);

            this.documentServiceMock.Verify(service =>
                service.CreateAndAssignAccessPoliciesAsync(
                    inputContainer,
                    It.Is(SameAccessPolicyListAs(inputAccessPolicies))),
                        Times.Once);

            this.documentServiceMock.Verify(service =>
                service.CreateSasTokenAsync(
                    inputContainer,
                    randomProjectStorageConfiguration.PickupFolder,
                    inputInboxPolicyname,
                    inputExpiresOn),
                        Times.Once);

            this.documentServiceMock.Verify(service =>
                service.CreateSasTokenAsync(
                    inputContainer,
                    randomProjectStorageConfiguration.LandingFolder,
                    inputOutboxPolicyname,
                    inputExpiresOn),
                        Times.Once);

            this.documentServiceMock.Verify(service =>
                service.CreateSasTokenAsync(
                    inputContainer,
                    randomProjectStorageConfiguration.ErrorFolder,
                    inputErrorsPolicyname,
                    inputExpiresOn),
                        Times.Once);

            this.documentServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
