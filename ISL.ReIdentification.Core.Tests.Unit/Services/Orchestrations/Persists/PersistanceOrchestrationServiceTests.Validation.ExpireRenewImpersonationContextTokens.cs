// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnExpireRenewImpersonationContextTokensWhenInvalidAndLogItAsync()
        {
            // given
            var invalidImpersonationContextId = Guid.Empty;

            var invalidArgumentPersistanceOrchestrationException =
                new InvalidArgumentPersistanceOrchestrationException(
                    message: "Invalid argument persistance orchestration exception, " +
                        "please correct the errors and try again.");

            invalidArgumentPersistanceOrchestrationException.AddData(
                key: "impersonationContextId",
                values: "Id is invalid");

            var expectedPersistanceOrchestrationValidationException =
            new PersistanceOrchestrationValidationException(
                message: "Persistance orchestration validation error occurred, please fix errors and try again.",
                innerException: invalidArgumentPersistanceOrchestrationException);

            // when
            ValueTask<AccessRequest> expireRenewImpersonationContextTokenTask = this.persistanceOrchestrationService
                    .ExpireRenewImpersonationContextTokensAsync(invalidImpersonationContextId);

            PersistanceOrchestrationValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<PersistanceOrchestrationValidationException>(
                    testCode: expireRenewImpersonationContextTokenTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedPersistanceOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPersistanceOrchestrationValidationException))),
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
