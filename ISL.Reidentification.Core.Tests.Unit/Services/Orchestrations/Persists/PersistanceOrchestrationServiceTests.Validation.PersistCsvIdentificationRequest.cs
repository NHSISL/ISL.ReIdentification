// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnPersistCsvIdentificationRequestWhenAccessRequestIsNullAndLogItAsync()
        {
            // given
            AccessRequest invalidAccessRequest = null;

            var nullAccessRequestPersistanceOrchestrationException =
                new NullAccessRequestException(
                    message: "Access request is null.");

            var expectedPersistanceOrchestrationValidationException =
                new PersistanceOrchestrationValidationException(
                    message: "Persistance orchestration validation error occurred, please fix errors and try again.",
                    innerException: nullAccessRequestPersistanceOrchestrationException);

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
            this.hashBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnPersistCsvIdentificationRequestWhenInvalidAndLogItAsync()
        {
            // given
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            AccessRequest invalidAccessRequest = randomAccessRequest;
            invalidAccessRequest.CsvIdentificationRequest = null;

            var invalidArgumentPersistanceOrchestrationException =
                new InvalidArgumentPersistanceOrchestrationException(
                    message: "Invalid argument persistance orchestration exception, " +
                        "please correct the errors and try again.");

            invalidArgumentPersistanceOrchestrationException.AddData(
                key: "csvIdentificationRequest",
                values: "AccessRequest is invalid");

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
            this.hashBrokerMock.VerifyNoOtherCalls();
        }
    }
}
