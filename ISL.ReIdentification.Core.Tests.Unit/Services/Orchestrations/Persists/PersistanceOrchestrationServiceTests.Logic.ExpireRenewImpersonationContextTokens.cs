// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Persists;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldExpireRenewImpersonationContextTokensAsync()
        {
            // given
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.CsvIdentificationRequest = null;
            randomAccessRequest.IdentificationRequest = null;
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            Guid inputImpersonationContextId = randomImpersonationContext.Id;
            ImpersonationContext outputImpersonationContext = randomImpersonationContext.DeepClone();

            AccessRequest inputAccessRequest = new AccessRequest
            {
                ImpersonationContext = outputImpersonationContext
            };

            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
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

            ProjectStorageConfiguration randomProjectStorageConfiguration =
                CreateRandomProjectStorageConfiguration();

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

            this.impersonationContextServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(inputImpersonationContextId))
                    .ReturnsAsync(outputImpersonationContext);

            persistanceOrchestrationServiceMock.Setup(service =>
                service.CreateOrRenewTokensAsync(It.Is(SameAccessRequestAs(inputAccessRequest))))
                    .ReturnsAsync(outputAccessRequest);

            PersistanceOrchestrationService service = persistanceOrchestrationServiceMock.Object;

            // when
            AccessRequest actualAccessRequest = await service
                .ExpireRenewImpersonationContextTokensAsync(inputImpersonationContextId);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.impersonationContextServiceMock.Verify(service =>
                service.RetrieveImpersonationContextByIdAsync(inputImpersonationContextId),
                    Times.Once);

            persistanceOrchestrationServiceMock.Verify(service =>
                service.CreateOrRenewTokensAsync(It.Is(SameAccessRequestAs(inputAccessRequest))),
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
