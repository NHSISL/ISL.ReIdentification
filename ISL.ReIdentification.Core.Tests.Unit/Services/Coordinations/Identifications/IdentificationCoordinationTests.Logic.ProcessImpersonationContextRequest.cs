// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
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
            string contextIdString = contextId.ToString();
            string randomString = GetRandomString();
            string container = GetRandomString();
            string fileName = randomString + ".csv";
            string filepath = $"/{container}/{contextIdString}/outbox/{fileName}";
            AccessRequest inputAccessRequest = CreateRandomAccessRequest();
            inputAccessRequest.CsvIdentificationRequest.Filepath = filepath;
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

            var identificationCoordinationServiceMock = new Mock<IdentificationCoordinationService>
                (this.accessOrchestrationServiceMock.Object,
                this.persistanceOrchestrationServiceMock.Object,
                this.identificationOrchestrationServiceMock.Object,
                this.csvHelperBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            { CallBase = true };

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    inputAccessRequest.CsvIdentificationRequest))
                        .ReturnsAsync(outputConversionIdentificationRequest);

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(contextId))
                    .ReturnsAsync(outputPersistanceOrchestrationAccessRequest);

            this.accessOrchestrationServiceMock.Setup(service =>
                service.ValidateAccessForIdentificationRequestAsync(It.IsAny<AccessRequest>()))
                    .ReturnsAsync(outputOrchestrationAccessRequest);

            this.identificationOrchestrationServiceMock.Setup(service =>
                service.ProcessIdentificationRequestAsync(It.IsAny<IdentificationRequest>()))
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
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    inputAccessRequest.CsvIdentificationRequest),
                    Times.Once);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveImpersonationContextByIdAsync(contextId),
                    Times.Once);

            this.accessOrchestrationServiceMock.Verify(service =>
                service.ValidateAccessForIdentificationRequestAsync(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.ProcessIdentificationRequestAsync(outputOrchestrationAccessRequest.IdentificationRequest),
                    Times.Once);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertIdentificationRequestToCsvIdentificationRequest(
                    outputOrchestrationIdentificationRequest),
                    Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.AddDocumentAsync(It.IsAny<Stream>(), fileName, container),
                    Times.Once);

            identificationCoordinationServiceMock.VerifyNoOtherCalls();
            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
