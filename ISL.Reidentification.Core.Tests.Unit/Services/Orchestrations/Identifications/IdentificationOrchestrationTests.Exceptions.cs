// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnProcessIdentificationRequestAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            int itemCount = GetRandomNumber();

            IdentificationRequest someIdentificationRequest =
               CreateRandomIdentificationRequest(hasAccess: false, itemCount: itemCount);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dependencyValidationException);

            var expectedIdentificationOrchestrationDependencyValidationException =
                new IdentificationOrchestrationDependencyValidationException(
                    message: "Identification orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<IdentificationRequest> identificationRequestTask =
                this.identificationOrchestrationService
                    .ProcessIdentificationRequestAsync(someIdentificationRequest);

            IdentificationOrchestrationDependencyValidationException
                actualIdentificationOrchestrationDependencyValidationException =
                await Assert.ThrowsAsync<IdentificationOrchestrationDependencyValidationException>(
                    testCode: identificationRequestTask.AsTask);

            // then
            actualIdentificationOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedIdentificationOrchestrationDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationOrchestrationDependencyValidationException))),
                       Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyOnProcessIdentificationRequestAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            int itemCount = GetRandomNumber();

            IdentificationRequest someIdentificationRequest =
               CreateRandomIdentificationRequest(hasAccess: false, itemCount: itemCount);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dependencyValidationException);

            var expectedIdentificationOrchestrationDependencyException =
                new IdentificationOrchestrationDependencyException(
                    message: "Identification orchestration dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<IdentificationRequest> identificationRequestTask =
                this.identificationOrchestrationService
                    .ProcessIdentificationRequestAsync(someIdentificationRequest);

            IdentificationOrchestrationDependencyException
                actualIdentificationOrchestrationDependencyException =
                await Assert.ThrowsAsync<IdentificationOrchestrationDependencyException>(
                    testCode: identificationRequestTask.AsTask);

            // then
            actualIdentificationOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedIdentificationOrchestrationDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationOrchestrationDependencyException))),
                       Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            int itemCount = GetRandomNumber();

            IdentificationRequest someIdentificationRequest =
               CreateRandomIdentificationRequest(hasAccess: false, itemCount: itemCount);

            var serviceException = new Exception();

            var failedServiceIdentificationOrchestrationException =
                new FailedServiceIdentificationOrchestrationException(
                    message: "Failed service identification orchestration error occurred, contact support.",
                    innerException: serviceException);

            var expectedIdentificationOrchestrationServiceException =
                new IdentificationOrchestrationServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceIdentificationOrchestrationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IdentificationRequest> identificationRequestTask =
                this.identificationOrchestrationService
                    .ProcessIdentificationRequestAsync(someIdentificationRequest);

            IdentificationOrchestrationServiceException
                actualIdentificationOrchestrationValidationException =
                await Assert.ThrowsAsync<IdentificationOrchestrationServiceException>(
                    testCode: identificationRequestTask.AsTask);

            // then
            actualIdentificationOrchestrationValidationException.Should().BeEquivalentTo(
                expectedIdentificationOrchestrationServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedIdentificationOrchestrationServiceException))),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationOrchestrationServiceException))),
                       Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
