// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(InvalidAccessRequestOnPersist))]
        public async Task ShouldThrowValidationExceptionOnPersisteCsvIdentificationRequestWhenInvalidAndLogItAsync(
            AccessRequest invalidAccessRequest)
        {
            // given
            var invalidArgumentPersistanceOrchestrationException =
                new InvalidArgumentPersistanceOrchestrationException(
                    message: "Invalid argument persistance orchestration exception, " +
                        "please correct the errors and try again.");

            invalidArgumentPersistanceOrchestrationException.AddData(
                key: "csvIdentificationRequestId",
                values: "Id is invalid");

            var expectedPersistanceOrchestrationValidationException =
            new PersistanceOrchestrationValidationException(
                message: "Persistance orchestration validation error occurred, please fix errors and try again.",
                innerException: invalidArgumentPersistanceOrchestrationException);

            // when
            ValueTask<AccessRequest> persistCsvIdentificationRequestAsyncTask =
                this.persistanceOrchestrationService.PersistCsvIdentificationRequestAsync(
                    invalidAccessRequest);

            PersistanceOrchestrationValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<PersistanceOrchestrationValidationException>(
                    testCode: persistCsvIdentificationRequestAsyncTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedPersistanceOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPersistanceOrchestrationValidationException))),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }
    }
}
