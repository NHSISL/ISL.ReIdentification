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
        public async Task ShouldThrowValidationExceptionOnRetrieveImpersonationContextWhenInvalidAndLogItAsync()
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
            ValueTask<AccessRequest> retrieveImpersonationContextByIdTask =
                this.persistanceOrchestrationService.RetrieveImpersonationContextByIdAsync(
                    invalidImpersonationContextId);

            PersistanceOrchestrationValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<PersistanceOrchestrationValidationException>(
                    testCode: retrieveImpersonationContextByIdTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedPersistanceOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPersistanceOrchestrationValidationException))),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
        }
    }
}
