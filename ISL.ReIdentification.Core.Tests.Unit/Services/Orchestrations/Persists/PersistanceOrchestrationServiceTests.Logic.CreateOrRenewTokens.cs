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
using ISL.ReIdentification.Core.Services.Orchestrations.Persists;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldCreatOrRenewTokenAsync()
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

            string inputInboxPolicyname = inputContainer + "-InboxPolicy";
            string inputOutboxPolicyname = inputContainer + "-OutboxPolicy";
            string inputErrorsPolicyname = inputContainer + "-ErrorsPolicy";

            List<AccessPolicy> inputAccessPolicies = GetAccessPolicies(
                inputInboxPolicyname,
                inputOutboxPolicyname,
                inputErrorsPolicyname);

            ProjectStorageConfiguration randomProjectStorageConfiguration =
                CreateRandomProjectStorageConfiguration();

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

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

            var persistanceOrchestrationServiceMock = new Mock<PersistanceOrchestrationService>(
                this.impersonationContextServiceMock.Object,
                this.csvIdentificationRequestServiceMock.Object,
                this.notificationServiceMock.Object,
                this.accessAuditServiceMock.Object,
                this.documentServiceMock.Object,
                this.loggingBrokerMock.Object,
                this.hashBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.identifierBrokerMock.Object,
                null,
                randomProjectStorageConfiguration)
            { CallBase = true };

            PersistanceOrchestrationService service = persistanceOrchestrationServiceMock.Object;

            // when
            AccessRequest actualAccessRequest = await service.CreateOrRenewTokensAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.documentServiceMock.Verify(service =>
                service.RetrieveListOfAllAccessPoliciesAsync(inputContainer),
                    Times.Once);

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

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.documentServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
