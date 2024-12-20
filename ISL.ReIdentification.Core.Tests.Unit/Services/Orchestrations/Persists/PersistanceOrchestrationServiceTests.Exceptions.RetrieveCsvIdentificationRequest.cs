// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnProcessPersistanceRequestAndLogItAsync(
                    Xeption dependencyValidationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dependencyValidationException);

            var expectedPersistanceOrchestrationDependencyValidationException =
                new PersistanceOrchestrationDependencyValidationException(
                    message: "Persistance orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<AccessRequest> retrieveCsvPersistanceRequestByIdTask =
                this.persistanceOrchestrationService
                    .RetrieveCsvIdentificationRequestByIdAsync(csvIdentificationRequestId: someId);

            PersistanceOrchestrationDependencyValidationException
                actualPersistanceOrchestrationDependencyValidationException =
                await Assert.ThrowsAsync<PersistanceOrchestrationDependencyValidationException>(
                    testCode: retrieveCsvPersistanceRequestByIdTask.AsTask);

            // then
            actualPersistanceOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedPersistanceOrchestrationDependencyValidationException);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPersistanceOrchestrationDependencyValidationException))),
                       Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.documentServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyOnProcessPersistanceRequestAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dependencyValidationException);

            var expectedPersistanceOrchestrationDependencyException =
                new PersistanceOrchestrationDependencyException(
                    message: "Persistance orchestration dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<AccessRequest> retrieveCsvPersistanceRequestByIdTask =
                this.persistanceOrchestrationService
                    .RetrieveCsvIdentificationRequestByIdAsync(someId);

            PersistanceOrchestrationDependencyException
                actualPersistanceOrchestrationDependencyException =
                await Assert.ThrowsAsync<PersistanceOrchestrationDependencyException>(
                    testCode: retrieveCsvPersistanceRequestByIdTask.AsTask);

            // then
            actualPersistanceOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedPersistanceOrchestrationDependencyException);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPersistanceOrchestrationDependencyException))),
                       Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.documentServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedServicePersistanceOrchestrationException =
                new FailedServicePersistanceOrchestrationException(
                    message: "Failed service persistance orchestration error occurred, contact support.",
                    innerException: serviceException);

            var expectedPersistanceOrchestrationServiceException =
                new PersistanceOrchestrationServiceException(
                    message: "Persistance orchestration service error occurred, contact support.",
                    innerException: failedServicePersistanceOrchestrationException);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<AccessRequest> retrieveCsvPersistanceRequestByIdTask =
                this.persistanceOrchestrationService
                    .RetrieveCsvIdentificationRequestByIdAsync(someId);

            PersistanceOrchestrationServiceException
                actualPersistanceOrchestrationValidationException =
                await Assert.ThrowsAsync<PersistanceOrchestrationServiceException>(
                    testCode: retrieveCsvPersistanceRequestByIdTask.AsTask);

            // then
            actualPersistanceOrchestrationValidationException.Should().BeEquivalentTo(
                expectedPersistanceOrchestrationServiceException);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPersistanceOrchestrationServiceException))),
                       Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.documentServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
