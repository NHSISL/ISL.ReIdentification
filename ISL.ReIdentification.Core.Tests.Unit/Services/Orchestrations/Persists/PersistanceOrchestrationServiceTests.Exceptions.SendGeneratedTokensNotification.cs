﻿// ---------------------------------------------------------
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
        [MemberData(nameof(ReturningNothingDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnSendGeneratedTokensNotificationAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            this.notificationServiceMock.Setup(service =>
                service.SendImpersonationTokensGeneratedNotificationAsync(someAccessRequest))
                    .ThrowsAsync(dependencyValidationException);

            var expectedPersistanceOrchestrationDependencyValidationException =
                new PersistanceOrchestrationDependencyValidationException(
                    message: "Persistance orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask sendGeneratedTokensNotificationTask = this.persistanceOrchestrationService
                .SendGeneratedTokensNotificationAsync(someAccessRequest);

            PersistanceOrchestrationDependencyValidationException
                actualPersistanceOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<PersistanceOrchestrationDependencyValidationException>(
                        testCode: sendGeneratedTokensNotificationTask.AsTask);

            // then
            actualPersistanceOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedPersistanceOrchestrationDependencyValidationException);

            this.notificationServiceMock.Verify(service => service
                .SendImpersonationTokensGeneratedNotificationAsync(It.Is(SameAccessRequestAs(someAccessRequest))),
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

        [Theory]
        [MemberData(nameof(ReturningNothingDependencyExceptions))]
        public async Task ShouldThrowDependencyOnSendGeneratedTokensNotificationAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            this.notificationServiceMock.Setup(service =>
                service.SendImpersonationTokensGeneratedNotificationAsync(someAccessRequest))
                    .ThrowsAsync(dependencyException);

            var expectedPersistanceOrchestrationDependencyException =
                new PersistanceOrchestrationDependencyException(
                    message: "Persistance orchestration dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            // when
            ValueTask sendGeneratedTokensNotificationTask = this.persistanceOrchestrationService
                .SendGeneratedTokensNotificationAsync(someAccessRequest);

            PersistanceOrchestrationDependencyException
                actualPersistanceOrchestrationDependencyException =
                    await Assert.ThrowsAsync<PersistanceOrchestrationDependencyException>(
                        testCode: sendGeneratedTokensNotificationTask.AsTask);

            // then
            actualPersistanceOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedPersistanceOrchestrationDependencyException);

            this.notificationServiceMock.Verify(service => service
                .SendImpersonationTokensGeneratedNotificationAsync(It.Is(SameAccessRequestAs(someAccessRequest))),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPersistanceOrchestrationDependencyException))),
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
        public async Task ShouldThrowServiceExceptionOnSendGeneratedTokensNotificationdAndLogItAsync()
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();
            var serviceException = new Exception();

            var failedServicePersistanceOrchestrationException =
                new FailedServicePersistanceOrchestrationException(
                    message: "Failed service persistance orchestration error occurred, contact support.",
                    innerException: serviceException);

            var expectedPersistanceOrchestrationServiceException =
                new PersistanceOrchestrationServiceException(
                    message: "Persistance orchestration service error occurred, contact support.",
                    innerException: failedServicePersistanceOrchestrationException);

            this.notificationServiceMock.Setup(service =>
                service.SendImpersonationTokensGeneratedNotificationAsync(someAccessRequest))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask sendGeneratedTokensNotificationTask = this.persistanceOrchestrationService
                 .SendGeneratedTokensNotificationAsync(someAccessRequest);

            PersistanceOrchestrationServiceException
                actualPersistanceOrchestrationValidationException =
                    await Assert.ThrowsAsync<PersistanceOrchestrationServiceException>(
                        testCode: sendGeneratedTokensNotificationTask.AsTask);

            // then
            actualPersistanceOrchestrationValidationException.Should()
                .BeEquivalentTo(expectedPersistanceOrchestrationServiceException);

            this.notificationServiceMock.Verify(service => service
                .SendImpersonationTokensGeneratedNotificationAsync(It.Is(SameAccessRequestAs(someAccessRequest))),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPersistanceOrchestrationServiceException))),
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
