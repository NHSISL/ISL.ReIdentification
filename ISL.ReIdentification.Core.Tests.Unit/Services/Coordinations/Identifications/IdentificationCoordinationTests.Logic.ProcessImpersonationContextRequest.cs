// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task ShouldProcessImpersonationContextRequestAsync()
        {
            // given
            Guid randomGuid = Guid.NewGuid();
            Guid contextId = randomGuid;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string timestamp = randomDateTimeOffset.ToString("yyyyMMddHHmms");
            string contextIdString = contextId.ToString();
            string randomString = GetRandomString();
            string container = GetRandomString();
            string fileName = randomString;
            string fileExtension = ".csv";
            string inputFilepath = $"/{container}/{contextIdString}/outbox/subdirectory/{fileName}{fileExtension}";
            string outputLandingFilepath = $"outbox/subdirectory/{fileName}{fileExtension}";
            string outputPickupFilepath = $"inbox/subdirectory/{fileName}_{timestamp}{fileExtension}";
            string outputErrorFilepath = $"error/subdirectory/{fileName}_{timestamp}{fileExtension}";
            Guid outputContextId = contextId;

            var outputExtractFromFilepath =
                (outputLandingFilepath, outputPickupFilepath, outputErrorFilepath, outputContextId);

            AccessRequest inputAccessRequest = CreateRandomAccessRequest();
            inputAccessRequest.CsvIdentificationRequest.Filepath = inputFilepath;
            AccessRequest outputPersistanceOrchestrationAccessRequest = CreateRandomAccessRequest();
            ImpersonationContext outputImpersonationContext = CreateRandomImpersonationContext();
            outputPersistanceOrchestrationAccessRequest.ImpersonationContext = outputImpersonationContext;
            AccessRequest outputConversionAccessRequest = CreateRandomAccessRequest();

            AccessRequest inputAccessOrchestrationAccessRequest = new AccessRequest
            {
                IdentificationRequest = outputConversionAccessRequest.IdentificationRequest,
            };

            AccessRequest outputOrchestrationAccessRequest = CreateRandomAccessRequest();
            IdentificationRequest outputOrchestrationIdentificationRequest = CreateRandomIdentificationRequest();
            CsvIdentificationRequest outputConversionCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            AccessRequest resultingAccessRequest = CreateRandomAccessRequest();
            resultingAccessRequest.CsvIdentificationRequest = outputConversionCsvIdentificationRequest;
            resultingAccessRequest.IdentificationRequest = null;
            resultingAccessRequest.ImpersonationContext = null;
            MemoryStream resultingCsvData = new MemoryStream(resultingAccessRequest.CsvIdentificationRequest.Data);
            AccessRequest expectedAccessRequest = resultingAccessRequest.DeepClone();

            var projectStorageConfiguration = new ProjectStorageConfiguration
            {
                Container = container,
                LandingFolder = GetRandomString(),
                PickupFolder = GetRandomString(),
                ErrorFolder = GetRandomString(),
            };

            var identificationCoordinationServiceMock = new Mock<IdentificationCoordinationService>
                (this.accessOrchestrationServiceMock.Object,
                this.persistanceOrchestrationServiceMock.Object,
                this.identificationOrchestrationServiceMock.Object,
                this.csvHelperBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                projectStorageConfiguration)
            { CallBase = true };

            identificationCoordinationServiceMock.Setup(service =>
                service.ExtractFromFilepath(inputFilepath))
                    .ReturnsAsync(outputExtractFromFilepath);

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    inputAccessRequest))
                        .ReturnsAsync(outputConversionAccessRequest);

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(contextId))
                    .ReturnsAsync(outputPersistanceOrchestrationAccessRequest);

            this.accessOrchestrationServiceMock.Setup(service =>
                service.ValidateAccessForIdentificationRequestAsync(
                    It.Is(SameAccessRequestAs(inputAccessOrchestrationAccessRequest))))
                        .ReturnsAsync(outputOrchestrationAccessRequest);

            this.identificationOrchestrationServiceMock.Setup(service =>
                service.ProcessIdentificationRequestAsync(outputOrchestrationAccessRequest.IdentificationRequest))
                    .ReturnsAsync(outputOrchestrationIdentificationRequest);

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertIdentificationRequestToCsvIdentificationRequest(
                    outputOrchestrationIdentificationRequest))
                        .ReturnsAsync(outputConversionCsvIdentificationRequest);

            IdentificationCoordinationService identificationCoordinationService =
                identificationCoordinationServiceMock.Object;

            // when
            AccessRequest actualAccessRequest =
                await identificationCoordinationService
                    .ProcessImpersonationContextRequestAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            identificationCoordinationServiceMock.Verify(service =>
                service.ExtractFromFilepath(inputFilepath),
                    Times.Once);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    inputAccessRequest),
                        Times.Once);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveImpersonationContextByIdAsync(contextId),
                    Times.Once);

            this.accessOrchestrationServiceMock.Verify(service =>
                service.ValidateAccessForIdentificationRequestAsync(
                    It.Is(SameAccessRequestAs(inputAccessOrchestrationAccessRequest))),
                        Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.ProcessIdentificationRequestAsync(outputOrchestrationAccessRequest.IdentificationRequest),
                    Times.Once);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertIdentificationRequestToCsvIdentificationRequest(
                    outputOrchestrationIdentificationRequest),
                        Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.AddDocumentAsync(It.Is(SameStreamAs(resultingCsvData)), outputPickupFilepath, container),
                    Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.RemoveDocumentByFileNameAsync(outputLandingFilepath, container),
                    Times.Once);

            identificationCoordinationServiceMock.VerifyNoOtherCalls();
            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldMoveFileToErrorFolderOnProcessImpersonationContextRequestOnErrorAsync()
        {
            // given
            Exception someException = new Exception();
            Guid randomGuid = Guid.NewGuid();
            Guid contextId = randomGuid;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string timestamp = randomDateTimeOffset.ToString("yyyyMMddHHmms");
            string contextIdString = contextId.ToString();
            string randomString = GetRandomString();
            string container = GetRandomString();
            string fileName = randomString;
            string fileExtension = ".csv";
            string inputFilepath = $"/{container}/{contextIdString}/outbox/subdirectory/{fileName}{fileExtension}";
            string outputLandingFilepath = $"outbox/subdirectory/{fileName}{fileExtension}";
            string outputPickupFilepath = $"inbox/subdirectory/{fileName}_{timestamp}{fileExtension}";
            string outputErrorFilepath = $"error/subdirectory/{fileName}_{timestamp}{fileExtension}";
            Guid outputContextId = contextId;

            var outputExtractFromFilepath =
                (outputLandingFilepath, outputPickupFilepath, outputErrorFilepath, outputContextId);

            AccessRequest inputAccessRequest = CreateRandomAccessRequest();
            inputAccessRequest.CsvIdentificationRequest.Filepath = inputFilepath;
            MemoryStream inputCsvData = new MemoryStream(inputAccessRequest.CsvIdentificationRequest.Data);
            AccessRequest outputPersistanceOrchestrationAccessRequest = CreateRandomAccessRequest();
            ImpersonationContext outputImpersonationContext = CreateRandomImpersonationContext();
            outputPersistanceOrchestrationAccessRequest.ImpersonationContext = outputImpersonationContext;
            IdentificationRequest outputConversionIdentificationRequest = CreateRandomIdentificationRequest();
            AccessRequest outputOrchestrationAccessRequest = CreateRandomAccessRequest();
            IdentificationRequest outputOrchestrationIdentificationRequest = CreateRandomIdentificationRequest();
            CsvIdentificationRequest outputConversionCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            AccessRequest resultingAccessRequest = CreateRandomAccessRequest();
            resultingAccessRequest.CsvIdentificationRequest = outputConversionCsvIdentificationRequest;
            resultingAccessRequest.IdentificationRequest = null;
            resultingAccessRequest.ImpersonationContext = null;
            AccessRequest expectedAccessRequest = resultingAccessRequest.DeepClone();

            var projectStorageConfiguration = new ProjectStorageConfiguration
            {
                Container = container,
                LandingFolder = GetRandomString(),
                PickupFolder = GetRandomString(),
                ErrorFolder = GetRandomString(),
            };

            var identificationCoordinationServiceMock = new Mock<IdentificationCoordinationService>
                (this.accessOrchestrationServiceMock.Object,
                this.persistanceOrchestrationServiceMock.Object,
                this.identificationOrchestrationServiceMock.Object,
                this.csvHelperBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                projectStorageConfiguration)
            { CallBase = true };

            var expectedIdentificationCoordinationServiceException =
                new IdentificationCoordinationServiceException(
                    message: "Identification coordination service error occurred, " +
                        "fix the errors and try again.",
                    innerException: someException);

            identificationCoordinationServiceMock.Setup(service =>
                service.ExtractFromFilepath(inputFilepath))
                    .ReturnsAsync(outputExtractFromFilepath);

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    inputAccessRequest))
                        .ThrowsAsync(someException);

            IdentificationCoordinationService identificationCoordinationService =
                identificationCoordinationServiceMock.Object;

            // when
            ValueTask<AccessRequest> accessRequestTask = identificationCoordinationService
                .ProcessImpersonationContextRequestAsync(inputAccessRequest);

            IdentificationCoordinationServiceException actualIdentificationCoordinationValidationException =
                await Assert.ThrowsAsync<IdentificationCoordinationServiceException>(
                    testCode: accessRequestTask.AsTask);

            // then
            identificationCoordinationServiceMock.Verify(service =>
                service.ExtractFromFilepath(inputFilepath),
                    Times.Once);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    inputAccessRequest),
                        Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.AddDocumentAsync(It.Is(SameStreamAs(inputCsvData)), outputErrorFilepath, container),
                    Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.RemoveDocumentByFileNameAsync(outputLandingFilepath, container),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationCoordinationServiceException))),
                       Times.Once);

            identificationCoordinationServiceMock.VerifyNoOtherCalls();
            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
