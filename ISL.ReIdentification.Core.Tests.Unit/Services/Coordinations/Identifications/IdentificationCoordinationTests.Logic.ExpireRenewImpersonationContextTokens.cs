// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldExpireRenewImpersonationContextTokensAsync(bool isPreviosulyApproved)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomFutureDateTimeOffset();
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.IdentificationRequest = null;
            randomAccessRequest.CsvIdentificationRequest = null;
            Guid inputImpersonationContextId = randomAccessRequest.ImpersonationContext.Id;
            AccessRequest retrievedAccessRequest = randomAccessRequest.DeepClone();
            retrievedAccessRequest.ImpersonationContext.IsApproved = isPreviosulyApproved;
            AccessRequest approvedAccessRequest = retrievedAccessRequest.DeepClone();

            if (!isPreviosulyApproved)
            {
                approvedAccessRequest.ImpersonationContext.IsApproved = true;
                approvedAccessRequest.ImpersonationContext.UpdatedDate = randomDateTimeOffset;
            }

            AccessRequest inputAccessRequest = approvedAccessRequest.DeepClone();
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(inputImpersonationContextId))
                    .ReturnsAsync(retrievedAccessRequest);

            if (!isPreviosulyApproved)
            {
                this.persistanceOrchestrationServiceMock.Setup(service =>
                    service.PersistImpersonationContextAsync(It.Is(SameAccessRequestAs(approvedAccessRequest))))
                        .ReturnsAsync(retrievedAccessRequest);

                this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffsetAsync())
                        .ReturnsAsync(randomDateTimeOffset);
            }

            this.identificationOrchestrationServiceMock.Setup(service =>
                service.ExpireRenewImpersonationContextTokensAsync(
                    It.Is(SameAccessRequestAs(inputAccessRequest)),
                    isPreviosulyApproved))
                        .ReturnsAsync(outputAccessRequest);

            // when
            AccessRequest actualAccessRequest = await this.identificationCoordinationService
                .ExpireRenewImpersonationContextTokensAsync(inputImpersonationContextId);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveImpersonationContextByIdAsync(inputImpersonationContextId),
                    Times.Once);

            if (!isPreviosulyApproved)
            {
                this.persistanceOrchestrationServiceMock.Verify(service =>
                   service.PersistImpersonationContextAsync(It.Is(SameAccessRequestAs(approvedAccessRequest))),
                        Times.Once);

                this.dateTimeBrokerMock.Verify(broker =>
                    broker.GetCurrentDateTimeOffsetAsync(),
                        Times.Once);
            }

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.ExpireRenewImpersonationContextTokensAsync(
                    It.Is(SameAccessRequestAs(inputAccessRequest)),
                    isPreviosulyApproved),
                        Times.Once);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.SendGeneratedTokensNotificationAsync(It.Is(SameAccessRequestAs(outputAccessRequest))),
                    Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
