// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions;
using ISL.ReIdentification.Core.Services.Orchestrations.Persists;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Fact]
        public async Task
            ShouldThrowCriticalValidationExceptionOnPurgeCsvReIdentificationRecordsThatExpiredWhenConfigurationIsNullAndLogItAsync()
        {
            // given
            var service = new PersistanceOrchestrationService(
                impersonationContextService: this.impersonationContextServiceMock.Object,
                csvIdentificationRequestService: this.csvIdentificationRequestServiceMock.Object,
                notificationService: this.notificationServiceMock.Object,
                accessAuditService: this.accessAuditServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                hashBroker: this.hashBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                identifierBroker: this.identifierBrokerMock.Object,
                csvReIdentificationConfigurations: null);

            var nullCsvReIdentificationConfigurationPersistanceOrchestrationException =
                new NullCsvReIdentificationConfigurationException(
                    message: "Csv reidentification configuration is null.");

            var expectedPersistanceOrchestrationValidationException =
                new PersistanceOrchestrationValidationException(
                    message: "Persistance orchestration validation error occurred, please fix errors and try again.",
                    innerException: nullCsvReIdentificationConfigurationPersistanceOrchestrationException);

            // when
            ValueTask purgeCsvIdentificationRequestAsyncTask =
                service.PurgeCsvReIdentificationRecordsThatExpired();

            PersistanceOrchestrationValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<PersistanceOrchestrationValidationException>(
                    testCode: purgeCsvIdentificationRequestAsyncTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedPersistanceOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
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

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnPurgeCsvReIdentificationRecordsThatExpiredWhenArgumentsIsInvalidAndLogItAsync()
        {
            // given
            var invalidExpireAfterMinutes = 3;

            var service = new PersistanceOrchestrationService(
                impersonationContextService: this.impersonationContextServiceMock.Object,
                csvIdentificationRequestService: this.csvIdentificationRequestServiceMock.Object,
                notificationService: this.notificationServiceMock.Object,
                accessAuditService: this.accessAuditServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                hashBroker: this.hashBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                identifierBroker: this.identifierBrokerMock.Object,

                csvReIdentificationConfigurations: new CsvReIdentificationConfigurations
                {
                    ExpireAfterMinutes = invalidExpireAfterMinutes
                });

            var invalidPersistanceOrchestrationException =
                new InvalidArgumentPersistanceOrchestrationException(
                    message: "Invalid argument persistance orchestration exception, please correct the errors and try again.");

            invalidPersistanceOrchestrationException.AddData(
                key: $"{nameof(CsvReIdentificationConfigurations)}.{nameof(CsvReIdentificationConfigurations.ExpireAfterMinutes)}",
                values: "Value is invalid");

            var expectedPersistanceOrchestrationValidationException =
                new PersistanceOrchestrationValidationException(
                    message: "Persistance orchestration validation error occurred, please fix errors and try again.",
                    innerException: invalidPersistanceOrchestrationException);

            // when
            ValueTask purgeCsvReIdentificationRecordsThatExpiredTask =
                service.PurgeCsvReIdentificationRecordsThatExpired();

            PersistanceOrchestrationValidationException actualPersistanceOrchestrationValidationException =
                await Assert.ThrowsAsync<PersistanceOrchestrationValidationException>(
                    testCode: purgeCsvReIdentificationRecordsThatExpiredTask.AsTask);

            // then
            actualPersistanceOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPersistanceOrchestrationValidationException);

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
