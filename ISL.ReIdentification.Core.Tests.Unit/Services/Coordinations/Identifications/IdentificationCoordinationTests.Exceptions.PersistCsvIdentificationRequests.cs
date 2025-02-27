// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnPersistsCsvIdentificationRequestAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            var identificationCoordinationServiceMock = new Mock<IdentificationCoordinationService>
                (this.accessOrchestrationServiceMock.Object,
                this.persistanceOrchestrationServiceMock.Object,
                this.identificationOrchestrationServiceMock.Object,
                this.csvHelperBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.projectStorageConfiguration)
            { CallBase = true };

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(It.IsAny<AccessRequest>()))
                    .ThrowsAsync(dependencyValidationException);

            var expectedIdentificationCoordinationDependencyValidationException =
                new IdentificationCoordinationDependencyValidationException(
                    message: "Identification coordination dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException);

            IdentificationCoordinationService identificationCoordinationService =
                identificationCoordinationServiceMock.Object;

            // when
            ValueTask<AccessRequest> accessRequestTask = identificationCoordinationService
                .PersistsCsvIdentificationRequestAsync(someAccessRequest);

            IdentificationCoordinationDependencyValidationException
                actualIdentificationCoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<IdentificationCoordinationDependencyValidationException>(
                        testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationDependencyValidationException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationDependencyValidationException);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationCoordinationDependencyValidationException))),
                       Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnPersistsCsvIdentificationRequestAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            var identificationCoordinationServiceMock = new Mock<IdentificationCoordinationService>
                (this.accessOrchestrationServiceMock.Object,
                this.persistanceOrchestrationServiceMock.Object,
                this.identificationOrchestrationServiceMock.Object,
                this.csvHelperBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.projectStorageConfiguration)
            { CallBase = true };

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(It.IsAny<AccessRequest>()))
                    .ThrowsAsync(dependencyException);

            var expectedIdentificationCoordinationDependencyException =
                new IdentificationCoordinationDependencyException(
                    message: "Identification coordination dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            IdentificationCoordinationService identificationCoordinationService =
                identificationCoordinationServiceMock.Object;

            // when
            ValueTask<AccessRequest> accessRequestTask = identificationCoordinationService
                .PersistsCsvIdentificationRequestAsync(someAccessRequest);

            IdentificationCoordinationDependencyException
                actualIdentificationCoordinationDependencyException =
                    await Assert.ThrowsAsync<IdentificationCoordinationDependencyException>(
                        testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationDependencyException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationDependencyException);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationCoordinationDependencyException))),
                       Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnPersistsCsvIdentificationRequestAndLogItAsync()
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();
            Exception someException = new Exception();

            var identificationCoordinationServiceMock = new Mock<IdentificationCoordinationService>
                (this.accessOrchestrationServiceMock.Object,
                this.persistanceOrchestrationServiceMock.Object,
                this.identificationOrchestrationServiceMock.Object,
                this.csvHelperBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.projectStorageConfiguration)
            { CallBase = true };

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(It.IsAny<AccessRequest>()))
                    .ThrowsAsync(someException);

            var expectedIdentificationCoordinationServiceException =
                new IdentificationCoordinationServiceException(
                    message: "Identification coordination service error occurred, " +
                        "fix the errors and try again.",
                    innerException: someException);

            IdentificationCoordinationService identificationCoordinationService =
                identificationCoordinationServiceMock.Object;

            // when
            ValueTask<AccessRequest> accessRequestTask = identificationCoordinationService
                .PersistsCsvIdentificationRequestAsync(someAccessRequest);

            IdentificationCoordinationServiceException
                actualIdentificationCoordinationServiceException =
                    await Assert.ThrowsAsync<IdentificationCoordinationServiceException>(
                        testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationServiceException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationServiceException);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedIdentificationCoordinationServiceException))),
                    Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
