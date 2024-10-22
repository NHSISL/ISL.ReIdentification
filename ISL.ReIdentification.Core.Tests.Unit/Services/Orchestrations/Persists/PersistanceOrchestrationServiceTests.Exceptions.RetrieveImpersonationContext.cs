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
        public async Task ShouldThrowDependencyValidationOnRetrieveImpersonationRequestAndLogItAsync(
                    Xeption dependencyValidationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            this.impersonationContextServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dependencyValidationException);

            var expectedPersistanceOrchestrationDependencyValidationException =
                new PersistanceOrchestrationDependencyValidationException(
                    message: "Persistance orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<AccessRequest> retrieveImpersonationContextByIdTask =
                this.persistanceOrchestrationService
                    .RetrieveImpersonationContextByIdAsync(impersonationContextId: someId);

            PersistanceOrchestrationDependencyValidationException
                actualPersistanceOrchestrationDependencyValidationException =
                await Assert.ThrowsAsync<PersistanceOrchestrationDependencyValidationException>(
                    testCode: retrieveImpersonationContextByIdTask.AsTask);

            // then
            actualPersistanceOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedPersistanceOrchestrationDependencyValidationException);

            this.impersonationContextServiceMock.Verify(service =>
                service.RetrieveImpersonationContextByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPersistanceOrchestrationDependencyValidationException))),
                       Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyOnRetrieveImpersonationContextAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            this.impersonationContextServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dependencyValidationException);

            var expectedPersistanceOrchestrationDependencyException =
                new PersistanceOrchestrationDependencyException(
                    message: "Persistance orchestration dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<AccessRequest> retrieveImpersonationContextByIdTask =
                this.persistanceOrchestrationService
                    .RetrieveImpersonationContextByIdAsync(someId);

            PersistanceOrchestrationDependencyException
                actualPersistanceOrchestrationDependencyException =
                await Assert.ThrowsAsync<PersistanceOrchestrationDependencyException>(
                    testCode: retrieveImpersonationContextByIdTask.AsTask);

            // then
            actualPersistanceOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedPersistanceOrchestrationDependencyException);

            this.impersonationContextServiceMock.Verify(service =>
                service.RetrieveImpersonationContextByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPersistanceOrchestrationDependencyException))),
                       Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveImpersonationContextIfServiceErrorOccurredAndLogItAsync()
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

            this.impersonationContextServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<AccessRequest> retrieveImpersonationContextByIdTask =
                this.persistanceOrchestrationService
                    .RetrieveImpersonationContextByIdAsync(someId);

            PersistanceOrchestrationServiceException
                actualPersistanceOrchestrationValidationException =
                await Assert.ThrowsAsync<PersistanceOrchestrationServiceException>(
                    testCode: retrieveImpersonationContextByIdTask.AsTask);

            // then
            actualPersistanceOrchestrationValidationException.Should().BeEquivalentTo(
                expectedPersistanceOrchestrationServiceException);

            this.impersonationContextServiceMock.Verify(service =>
                service.RetrieveImpersonationContextByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPersistanceOrchestrationServiceException))),
                       Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
        }
    }
}
