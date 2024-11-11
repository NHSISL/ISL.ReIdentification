// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnProcessImpersonationContextWhenAccessRequestIsNullAndLogItAsync()
        {
            // given
            var service = new IdentificationCoordinationService(
                accessOrchestrationService: this.accessOrchestrationServiceMock.Object,
                persistanceOrchestrationService: this.persistanceOrchestrationServiceMock.Object,
                identificationOrchestrationService: this.identificationOrchestrationServiceMock.Object,
                csvHelperBroker: this.csvHelperBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                identifierBroker: this.identifierBrokerMock.Object,
                projectStorageConfiguration: null);

            AccessRequest nullAccessRequest = null;

            var invalidIdentificationCoordinationException =
                new InvalidIdentificationCoordinationException(
                    message: "Invalid identification coordination exception. Please correct the errors and try again.");

            invalidIdentificationCoordinationException.AddData(
                key: nameof(AccessRequest),
                values: "Object is invalid");

            invalidIdentificationCoordinationException.AddData(
                key: nameof(ProjectStorageConfiguration),
                values: "Object is invalid");

            var expectedIdentificationCoordinationValidationException =
                new IdentificationCoordinationValidationException(
                    message: "Identification coordination validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: invalidIdentificationCoordinationException);

            // when
            ValueTask<AccessRequest> accessRequestTask = service
                .ProcessImpersonationContextRequestAsync(nullAccessRequest);

            IdentificationCoordinationValidationException actualIdentificationCoordinationValidationException =
                await Assert.ThrowsAsync<IdentificationCoordinationValidationException>(
                    testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationValidationException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationValidationException);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationCoordinationValidationException))),
                       Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task
            ShouldThrowValidationExceptionOnProcessImpersonationContextWhenArgumentsIsInvalidAndLogItAsync(
            string invalidString)
        {
            // given
            var service = new IdentificationCoordinationService(
                accessOrchestrationService: this.accessOrchestrationServiceMock.Object,
                persistanceOrchestrationService: this.persistanceOrchestrationServiceMock.Object,
                identificationOrchestrationService: this.identificationOrchestrationServiceMock.Object,
                csvHelperBroker: this.csvHelperBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                identifierBroker: this.identifierBrokerMock.Object,
                projectStorageConfiguration: new ProjectStorageConfiguration
                {
                    Container = invalidString,
                    LandingFolder = invalidString,
                    PickupFolder = invalidString,
                    ErrorFolder = invalidString,
                });

            AccessRequest nullAccessRequest = new AccessRequest
            {
                CsvIdentificationRequest = null
            };

            var invalidIdentificationCoordinationException =
                new InvalidIdentificationCoordinationException(
                    message: "Invalid identification coordination exception. Please correct the errors and try again.");

            invalidIdentificationCoordinationException.AddData(
                key: $"{nameof(AccessRequest)}.{nameof(AccessRequest.CsvIdentificationRequest)}",
                values: "Object is invalid");

            invalidIdentificationCoordinationException.AddData(
                key: $"{nameof(ProjectStorageConfiguration)}.{nameof(ProjectStorageConfiguration.Container)}",
                values: "Text is invalid");

            invalidIdentificationCoordinationException.AddData(
                key: $"{nameof(ProjectStorageConfiguration)}.{nameof(ProjectStorageConfiguration.PickupFolder)}",
                values: "Text is invalid");

            invalidIdentificationCoordinationException.AddData(
                key: $"{nameof(ProjectStorageConfiguration)}.{nameof(ProjectStorageConfiguration.LandingFolder)}",
                values: "Text is invalid");

            invalidIdentificationCoordinationException.AddData(
                key: $"{nameof(ProjectStorageConfiguration)}.{nameof(ProjectStorageConfiguration.ErrorFolder)}",
                values: "Text is invalid");

            var expectedIdentificationCoordinationValidationException =
                new IdentificationCoordinationValidationException(
                    message: "Identification coordination validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: invalidIdentificationCoordinationException);

            // when
            ValueTask<AccessRequest> accessRequestTask =
                service.ProcessImpersonationContextRequestAsync(nullAccessRequest);

            IdentificationCoordinationValidationException actualIdentificationCoordinationValidationException =
                await Assert.ThrowsAsync<IdentificationCoordinationValidationException>(
                    testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationValidationException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationValidationException);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationCoordinationValidationException))),
                       Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task
            ShouldThrowValidationExceptionOnProcessImpersonationContextWhenFilepathIsInvalidAndLogItAsync(
            string invalidString)
        {
            // given
            AccessRequest nullAccessRequest = new AccessRequest
            {
                CsvIdentificationRequest = new CsvIdentificationRequest { Filepath = invalidString }
            };

            var invalidIdentificationCoordinationException =
                new InvalidIdentificationCoordinationException(
                    message: "Invalid identification coordination exception. Please correct the errors and try again.");

            invalidIdentificationCoordinationException.AddData(
                key:
                    $"{nameof(AccessRequest)}" +
                    $".{nameof(AccessRequest.CsvIdentificationRequest)}" +
                    $".{nameof(AccessRequest.CsvIdentificationRequest.Filepath)}",
                values: "Text is invalid");

            var expectedIdentificationCoordinationValidationException =
                new IdentificationCoordinationValidationException(
                    message: "Identification coordination validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: invalidIdentificationCoordinationException);

            // when
            ValueTask<AccessRequest> accessRequestTask = this.identificationCoordinationService
                .ProcessImpersonationContextRequestAsync(nullAccessRequest);

            IdentificationCoordinationValidationException actualIdentificationCoordinationValidationException =
                await Assert.ThrowsAsync<IdentificationCoordinationValidationException>(
                    testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationValidationException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationValidationException);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationCoordinationValidationException))),
                       Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
