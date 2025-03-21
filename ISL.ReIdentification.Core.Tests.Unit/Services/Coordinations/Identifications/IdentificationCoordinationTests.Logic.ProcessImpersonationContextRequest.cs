﻿// ---------------------------------------------------------
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
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
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
            string inputContainer = GetRandomString();
            string fileName = randomString;
            string fileExtension = ".csv";
            string inputFilepath = $"outbox/subdirectory/{fileName}{fileExtension}";
            string outputLandingFilepath = $"outbox/subdirectory/{fileName}{fileExtension}";
            string outputPickupFilepath = $"inbox/subdirectory/{fileName}_{timestamp}{fileExtension}";
            string outputErrorFilepath = $"error/subdirectory/{fileName}_{timestamp}{fileExtension}";
            Guid outputContextId = contextId;

            var outputExtractFromFilepath =
                (outputLandingFilepath, outputPickupFilepath, outputErrorFilepath);

            AccessRequest outputAccessRequestWithCsvIdentificationRequest = CreateRandomAccessRequest();
            outputAccessRequestWithCsvIdentificationRequest.CsvIdentificationRequest.Filepath = inputFilepath;
            outputAccessRequestWithCsvIdentificationRequest.IdentificationRequest = null;
            AccessRequest outputConversionAccessRequest = outputAccessRequestWithCsvIdentificationRequest.DeepClone();
            outputConversionAccessRequest.IdentificationRequest = CreateRandomIdentificationRequest();
            AccessRequest outputPersistanceOrchestrationAccessRequest = CreateRandomAccessRequest();
            outputPersistanceOrchestrationAccessRequest.IdentificationRequest = null;
            outputPersistanceOrchestrationAccessRequest.CsvIdentificationRequest = null;
            AccessRequest inputAccessOrchestrationAccessRequest = outputConversionAccessRequest;
            AccessRequest outputOrchestrationAccessRequest = inputAccessOrchestrationAccessRequest.DeepClone();
            AccessRequest inputIdentificationOrchestrationAccessRequest = outputOrchestrationAccessRequest.DeepClone();
            IdentificationRequest outputOrchestrationIdentificationRequest = CreateRandomIdentificationRequest();
            AccessRequest inputConversionAccessRequest = outputOrchestrationAccessRequest.DeepClone();
            inputConversionAccessRequest.IdentificationRequest = outputOrchestrationIdentificationRequest;
            AccessRequest resultingAccessRequest = inputConversionAccessRequest.DeepClone();
            resultingAccessRequest.CsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            MemoryStream resultingCsvData = new MemoryStream(resultingAccessRequest.CsvIdentificationRequest.Data);
            AccessRequest expectedAccessRequest = resultingAccessRequest.DeepClone();

            var projectStorageConfiguration = new ProjectStorageConfiguration
            {
                Container = inputContainer,
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
                service.CreateAccessRequestWithCsvIdentificationRequestAsync(
                    inputContainer,
                    inputFilepath,
                    outputErrorFilepath))
                        .ReturnsAsync(outputAccessRequestWithCsvIdentificationRequest);

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    It.Is(SameAccessRequestAs(outputAccessRequestWithCsvIdentificationRequest))))
                        .ReturnsAsync(outputConversionAccessRequest);

            this.accessOrchestrationServiceMock.Setup(service =>
                service.ValidateAccessForIdentificationRequestAsync(
                    It.Is(SameAccessRequestAs(inputAccessOrchestrationAccessRequest))))
                        .ReturnsAsync(outputOrchestrationAccessRequest);

            this.identificationOrchestrationServiceMock.Setup(service =>
                service.ProcessIdentificationRequestAsync(
                        It.Is(SameIdentificationRequestAs(
                            inputIdentificationOrchestrationAccessRequest.IdentificationRequest))))
                    .ReturnsAsync(outputOrchestrationIdentificationRequest);

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertIdentificationRequestToCsvIdentificationRequest(
                    It.Is(SameAccessRequestAs(inputConversionAccessRequest))))
                        .ReturnsAsync(resultingAccessRequest);

            IdentificationCoordinationService identificationCoordinationService =
                identificationCoordinationServiceMock.Object;

            // when
            AccessRequest actualAccessRequest =
                await identificationCoordinationService
                    .ProcessImpersonationContextRequestAsync(inputContainer, inputFilepath);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            identificationCoordinationServiceMock.Verify(service =>
                service.ExtractFromFilepath(inputFilepath),
                    Times.Once);

            identificationCoordinationServiceMock.Verify(service =>
                service.CreateAccessRequestWithCsvIdentificationRequestAsync(
                    inputContainer,
                    inputFilepath,
                    outputErrorFilepath),
                        Times.Once);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    It.Is(SameAccessRequestAs(outputAccessRequestWithCsvIdentificationRequest))),
                        Times.Once);

            this.accessOrchestrationServiceMock.Verify(service =>
                service.ValidateAccessForIdentificationRequestAsync(
                    It.Is(SameAccessRequestAs(inputAccessOrchestrationAccessRequest))),
                        Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.ProcessIdentificationRequestAsync(It.Is(SameIdentificationRequestAs(
                            inputIdentificationOrchestrationAccessRequest.IdentificationRequest))),
                    Times.Once);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertIdentificationRequestToCsvIdentificationRequest(
                    It.Is(SameAccessRequestAs(inputConversionAccessRequest))),
                        Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.AddDocumentAsync(It.Is(SameStreamAs(resultingCsvData)), outputPickupFilepath, inputContainer),
                    Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.RemoveDocumentByFileNameAsync(outputLandingFilepath, inputContainer),
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
            string inputFilepath = $"outbox/subdirectory/{fileName}{fileExtension}";
            string outputLandingFilepath = $"outbox/subdirectory/{fileName}{fileExtension}";
            string outputPickupFilepath = $"inbox/subdirectory/{fileName}_{timestamp}{fileExtension}";
            string outputErrorFilepath = $"error/subdirectory/{fileName}_{timestamp}{fileExtension}";
            Guid outputContextId = contextId;

            var outputExtractFromFilepath =
                (outputLandingFilepath, outputPickupFilepath, outputErrorFilepath);

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
                service.CreateAccessRequestWithCsvIdentificationRequestAsync(
                    container,
                    inputFilepath,
                    outputErrorFilepath))
                        .ReturnsAsync(inputAccessRequest);

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    It.Is(SameAccessRequestAs(inputAccessRequest))))
                        .ThrowsAsync(someException);

            IdentificationCoordinationService identificationCoordinationService =
                identificationCoordinationServiceMock.Object;

            // when
            ValueTask<AccessRequest> accessRequestTask = identificationCoordinationService
                .ProcessImpersonationContextRequestAsync(container, inputFilepath);

            IdentificationCoordinationServiceException actualIdentificationCoordinationValidationException =
                await Assert.ThrowsAsync<IdentificationCoordinationServiceException>(
                    testCode: accessRequestTask.AsTask);

            // then
            identificationCoordinationServiceMock.Verify(service =>
                service.ExtractFromFilepath(inputFilepath),
                    Times.Once);

            identificationCoordinationServiceMock.Verify(service =>
                service.CreateAccessRequestWithCsvIdentificationRequestAsync(
                    container,
                    inputFilepath,
                    outputErrorFilepath),
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
