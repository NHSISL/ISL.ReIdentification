// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions;
using ISL.ReIdentification.Core.Services.Orchestrations.Persists;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnPurgeCsvReIdentificationRecordsThatExpiredWhenConfigurationIsNullAndLogItAsync()
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
