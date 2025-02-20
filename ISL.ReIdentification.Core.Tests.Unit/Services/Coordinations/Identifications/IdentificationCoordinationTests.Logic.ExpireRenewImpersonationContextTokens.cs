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
        [Fact]
        public async Task ShouldExpireRenewImpersonationContextTokensWhenIsApprovedAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomFutureDateTimeOffset();
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.IdentificationRequest = null;
            randomAccessRequest.CsvIdentificationRequest = null;
            Guid inputImpersonationContextId = randomAccessRequest.ImpersonationContext.Id;
            AccessRequest retrievedAccessRequest = randomAccessRequest.DeepClone();
            retrievedAccessRequest.ImpersonationContext.IsApproved = true;
            AccessRequest approvedAccessRequest = retrievedAccessRequest.DeepClone();
            AccessRequest inputAccessRequest = approvedAccessRequest.DeepClone();
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(inputImpersonationContextId))
                    .ReturnsAsync(retrievedAccessRequest);

            this.identificationOrchestrationServiceMock.Setup(service =>
                service.ExpireRenewImpersonationContextTokensAsync(
                    It.Is(SameAccessRequestAs(inputAccessRequest))))
                        .ReturnsAsync(outputAccessRequest);

            // when
            AccessRequest actualAccessRequest = await this.identificationCoordinationService
                .ExpireRenewImpersonationContextTokensAsync(inputImpersonationContextId);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveImpersonationContextByIdAsync(inputImpersonationContextId),
                    Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.ExpireRenewImpersonationContextTokensAsync(
                    It.Is(SameAccessRequestAs(inputAccessRequest))),
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
