// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(ReturningNothingDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnSendApprovalNotificationAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();
            someAccessRequest.ImpersonationContext.IsApproved = true;

            this.notificationServiceMock.Setup(service =>
                service.SendImpersonationApprovedNotificationAsync(someAccessRequest))
                    .ThrowsAsync(dependencyValidationException);

            var expectedPersistanceOrchestrationDependencyValidationException =
                new PersistanceOrchestrationDependencyValidationException(
                    message: "Persistance orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask sendApprovalNotificationTask = this.persistanceOrchestrationService
                .SendApprovalNotificationAsync(someAccessRequest);

            PersistanceOrchestrationDependencyValidationException
                actualPersistanceOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<PersistanceOrchestrationDependencyValidationException>(
                        testCode: sendApprovalNotificationTask.AsTask);

            // then
            actualPersistanceOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedPersistanceOrchestrationDependencyValidationException);

            this.notificationServiceMock.Verify(service => service
                .SendImpersonationApprovedNotificationAsync(It.Is(SameAccessRequestAs(someAccessRequest))),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPersistanceOrchestrationDependencyValidationException))),
                       Times.Once);

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
