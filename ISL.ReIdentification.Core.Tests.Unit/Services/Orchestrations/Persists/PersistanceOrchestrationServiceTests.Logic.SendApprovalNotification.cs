// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldSendApprovalNotificationAsync(bool isApproved)
        {
            // given
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            inputAccessRequest.ImpersonationContext.IsApproved = isApproved;

            // when
            await this.persistanceOrchestrationService
                .SendApprovalNotificationAsync(inputAccessRequest);

            // then

            if (isApproved)
            {
                this.notificationServiceMock.Verify(service => service
                    .SendImpersonationApprovedNotificationAsync(It.Is(SameAccessRequestAs(inputAccessRequest))),
                    Times.Once);
            }
            else
            {
                this.notificationServiceMock.Verify(service => service
                    .SendImpersonationDeniedNotificationAsync(It.Is(SameAccessRequestAs(inputAccessRequest))),
                    Times.Once);
            }


            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
