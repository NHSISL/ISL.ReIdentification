﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnProcessCsvIdentificationRequestAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            Guid someCsvIdentificationRequestId = Guid.NewGuid();
            string someReason = GetRandomString();

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId))
                    .ThrowsAsync(dependencyValidationException);

            var expectedIdentificationCoordinationDependencyValidationException =
                new IdentificationCoordinationDependencyValidationException(
                    message: "Identification coordination dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException);

            // when
            ValueTask<AccessRequest> accessRequestTask =
                this.identificationCoordinationService
                    .ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId, someReason);

            IdentificationCoordinationDependencyValidationException
                actualIdentificationCoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<IdentificationCoordinationDependencyValidationException>(
                        testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationDependencyValidationException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationDependencyValidationException);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationCoordinationDependencyValidationException))),
                       Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyValidationExceptionOnProcessCsvIdentificationRequestWhenUnauthorizedAndLogItAsync()
        {
            // given
            Guid someCsvIdentificationRequestId = Guid.NewGuid();
            string someReason = GetRandomString();
            string someMessage = GetRandomString();

            var unauthorizedAccessOrchestrationException =
                new UnauthorizedAccessOrchestrationException(message: someMessage);

            var accessOrchestrationValidationException = new AccessOrchestrationValidationException(
                message: someMessage,
                innerException: unauthorizedAccessOrchestrationException);

            var unauthorizedIdentificationCoordinationException =
                new UnauthorizedIdentificationCoordinationException(
                    message: "Not authorised to perform this action",
                    innerException: accessOrchestrationValidationException.InnerException as Xeption);

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId))
                    .ThrowsAsync(accessOrchestrationValidationException);

            var expectedIdentificationCoordinationDependencyValidationException =
                new IdentificationCoordinationDependencyValidationException(
                    message: "Identification coordination dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: unauthorizedIdentificationCoordinationException);

            // when
            ValueTask<AccessRequest> accessRequestTask =
                this.identificationCoordinationService
                    .ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId, someReason);

            IdentificationCoordinationDependencyValidationException
                actualIdentificationCoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<IdentificationCoordinationDependencyValidationException>(
                        testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationDependencyValidationException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationDependencyValidationException);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationCoordinationDependencyValidationException))),
                       Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnProcessCsvIdentificationRequestAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            Guid someCsvIdentificationRequestId = Guid.NewGuid();
            string someReason = GetRandomString();

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId))
                    .ThrowsAsync(dependencyException);

            var expectedIdentificationCoordinationDependencyException =
                new IdentificationCoordinationDependencyException(
                    message: "Identification coordination dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            // when
            ValueTask<AccessRequest> accessRequestTask =
                this.identificationCoordinationService
                    .ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId, someReason);

            IdentificationCoordinationDependencyException
                actualIdentificationCoordinationDependencyException =
                    await Assert.ThrowsAsync<IdentificationCoordinationDependencyException>(
                        testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationDependencyException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationDependencyException);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationCoordinationDependencyException))),
                       Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnProcessCsvIdentificationRequestAndLogItAsync()
        {
            // given
            Guid someCsvIdentificationRequestId = Guid.NewGuid();
            string someReason = GetRandomString();
            Exception someException = new Exception();

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId))
                    .ThrowsAsync(someException);

            var expectedIdentificationCoordinationServiceException =
                new IdentificationCoordinationServiceException(
                    message: "Identification coordination service error occurred, " +
                        "fix the errors and try again.",
                    innerException: someException);

            // when
            ValueTask<AccessRequest> accessRequestTask =
                this.identificationCoordinationService
                    .ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId, someReason);

            IdentificationCoordinationServiceException
                actualIdentificationCoordinationServiceException =
                    await Assert.ThrowsAsync<IdentificationCoordinationServiceException>(
                        testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationServiceException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationServiceException);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedIdentificationCoordinationServiceException))),
                    Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
