// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldUpdateApprovalStatusImpersonationContextApprovalAsync(bool isApproved)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomFutureDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            EntraUser currentEntraUser = randomEntraUser;
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.IdentificationRequest = null;
            randomAccessRequest.CsvIdentificationRequest = null;
            randomAccessRequest.ImpersonationContext.ResponsiblePersonEntraUserId = currentEntraUser.EntraUserId;
            Guid inputImpersonationContextId = randomAccessRequest.ImpersonationContext.Id;
            AccessRequest retrievedAccessRequest = randomAccessRequest.DeepClone();
            retrievedAccessRequest.ImpersonationContext.IsApproved = !isApproved;
            AccessRequest updatedAccessRequest = retrievedAccessRequest.DeepClone();
            updatedAccessRequest.ImpersonationContext.IsApproved = isApproved;
            updatedAccessRequest.ImpersonationContext.UpdatedDate = randomDateTimeOffset;
            AccessRequest tokensAccessRequest = updatedAccessRequest.DeepClone();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(currentEntraUser);

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(inputImpersonationContextId))
                    .ReturnsAsync(retrievedAccessRequest);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffsetAsync())
                        .ReturnsAsync(randomDateTimeOffset);

            if (isApproved == false)
            {
                this.identificationOrchestrationServiceMock.Setup(service =>
                    service.ExpireRenewImpersonationContextTokensAsync(
                        It.Is(SameAccessRequestAs(updatedAccessRequest)),
                        true))
                            .ReturnsAsync(tokensAccessRequest);
            }

            // when
            await this.identificationCoordinationService
                .ImpersonationContextApprovalAsync(inputImpersonationContextId, isApproved);

            // then
            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                    broker.GetCurrentDateTimeOffsetAsync(),
                        Times.Once);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.PersistImpersonationContextAsync(It.Is(SameAccessRequestAs(updatedAccessRequest))),
                    Times.Once);

            if (isApproved == true)
            {
                this.persistanceOrchestrationServiceMock.Verify(service =>
                    service.SendApprovalNotificationAsync(It.Is(SameAccessRequestAs(updatedAccessRequest))),
                        Times.Once);
            }

            if (isApproved == false)
            {
                this.identificationOrchestrationServiceMock.Verify(service =>
                    service.ExpireRenewImpersonationContextTokensAsync(
                        It.Is(SameAccessRequestAs(updatedAccessRequest)),
                        true),
                            Times.Once);

                this.persistanceOrchestrationServiceMock.Verify(service =>
                    service.SendApprovalNotificationAsync(It.Is(SameAccessRequestAs(tokensAccessRequest))),
                        Times.Once);
            }

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
