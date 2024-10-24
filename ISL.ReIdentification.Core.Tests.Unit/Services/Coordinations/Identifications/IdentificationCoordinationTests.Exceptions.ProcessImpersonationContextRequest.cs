// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnProcessImpersonationContextRequestAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();
            Guid randomGuid = Guid.NewGuid();
            Guid contextId = randomGuid;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string timestamp = randomDateTimeOffset.ToString("yyyyMMddHHmms");
            string contextIdString = contextId.ToString();
            string randomString = GetRandomString();
            string container = GetRandomString();
            string fileName = randomString;
            string fileExtension = ".csv";
            string filepath = $"/{container}/{contextIdString}/outbox/subdirectory/{fileName}{fileExtension}";
            someAccessRequest.CsvIdentificationRequest.Filepath = filepath;
            string inputFileName = $"outbox/subdirectory/{fileName}{fileExtension}";
            string errorFileName = $"error/subdirectory/{fileName}_{timestamp}{fileExtension}";

            var identificationCoordinationServiceMock = new Mock<IdentificationCoordinationService>
                (this.accessOrchestrationServiceMock.Object,
                this.persistanceOrchestrationServiceMock.Object,
                this.identificationOrchestrationServiceMock.Object,
                this.csvHelperBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object)
            { CallBase = true };

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    someAccessRequest.CsvIdentificationRequest))
                        .ThrowsAsync(dependencyValidationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            var expectedIdentificationCoordinationDependencyValidationException =
                new IdentificationCoordinationDependencyValidationException(
                    message: "Identification coordination dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            IdentificationCoordinationService identificationCoordinationService =
                identificationCoordinationServiceMock.Object;

            // when
            ValueTask<AccessRequest> accessRequestTask =
                identificationCoordinationService
                    .ProcessImpersonationContextRequestAsync(someAccessRequest);

            IdentificationCoordinationDependencyValidationException
                actualIdentificationCoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<IdentificationCoordinationDependencyValidationException>(
                        testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationDependencyValidationException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationDependencyValidationException);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    someAccessRequest.CsvIdentificationRequest),
                        Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.AddDocumentAsync(It.IsAny<Stream>(), errorFileName, container),
                    Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.RemoveDocumentByFileNameAsync(inputFileName, container),
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
        public async Task ShouldThrowDependencyExceptionOnProcessImpersonationContextRequestAndLogItAsync(
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
                this.dateTimeBrokerMock.Object)
            { CallBase = true };

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    someAccessRequest.CsvIdentificationRequest))
                        .ThrowsAsync(dependencyException);

            var expectedIdentificationCoordinationDependencyException =
                new IdentificationCoordinationDependencyException(
                    message: "Identification coordination dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            IdentificationCoordinationService identificationCoordinationService =
                identificationCoordinationServiceMock.Object;

            // when
            ValueTask<AccessRequest> accessRequestTask =
                identificationCoordinationService
                    .ProcessImpersonationContextRequestAsync(someAccessRequest);

            IdentificationCoordinationDependencyException
                actualIdentificationCoordinationDependencyException =
                    await Assert.ThrowsAsync<IdentificationCoordinationDependencyException>(
                        testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationDependencyException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationDependencyException);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    someAccessRequest.CsvIdentificationRequest),
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
        public async Task ShouldThrowServiceExceptionOnProcessImpersonationContextRequestAndLogItAsync()
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
                this.dateTimeBrokerMock.Object)
            { CallBase = true };

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    someAccessRequest.CsvIdentificationRequest))
                        .ThrowsAsync(someException);

            var expectedIdentificationCoordinationServiceException =
                new IdentificationCoordinationServiceException(
                    message: "Identification coordination service error occurred, " +
                        "fix the errors and try again.",
                    innerException: someException);

            IdentificationCoordinationService identificationCoordinationService =
                identificationCoordinationServiceMock.Object;

            // when
            ValueTask<AccessRequest> accessRequestTask =
                identificationCoordinationService
                    .ProcessImpersonationContextRequestAsync(someAccessRequest);

            IdentificationCoordinationServiceException
                actualIdentificationCoordinationServiceException =
                    await Assert.ThrowsAsync<IdentificationCoordinationServiceException>(
                        testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationServiceException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationServiceException);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    someAccessRequest.CsvIdentificationRequest),
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
