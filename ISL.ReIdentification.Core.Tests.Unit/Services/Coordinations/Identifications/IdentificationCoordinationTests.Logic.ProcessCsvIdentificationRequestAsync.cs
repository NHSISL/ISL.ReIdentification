// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task ShouldProcessCsvIdentificationRequestAsync()
        {
            // given
            var identificationCoordinationServiceMock = new Mock<IdentificationCoordinationService>
                (this.accessOrchestrationServiceMock.Object,
                this.persistanceOrchestrationServiceMock.Object,
                this.identificationOrchestrationServiceMock.Object,
                this.csvHelperBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            { CallBase = true };

            Guid inputCsvIdentificationRequestId = Guid.NewGuid();
            AccessRequest outputPersistanceOrchestrationAccessRequest = CreateRandomAccessRequest(); ;
            IdentificationRequest outputConversionIdentificationRequest = CreateRandomIdentificationRequest();
            EntraUser outputEntraUser = CreateRandomEntraUser();
            AccessRequest outputOrchestrationAccessRequest = CreateRandomAccessRequest();
            IdentificationRequest outputOrchestrationIdentificationRequest = CreateRandomIdentificationRequest();
            CsvIdentificationRequest outputConversionCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            AccessRequest resultingAccessRequest = CreateRandomAccessRequest();
            resultingAccessRequest.CsvIdentificationRequest = outputConversionCsvIdentificationRequest;
            resultingAccessRequest.IdentificationRequest = null;
            resultingAccessRequest.ImpersonationContext = null;
            AccessRequest expectedAccessRequest = resultingAccessRequest.DeepClone();

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId))
                    .ReturnsAsync(outputPersistanceOrchestrationAccessRequest);

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    outputPersistanceOrchestrationAccessRequest.CsvIdentificationRequest))
                .ReturnsAsync(outputConversionIdentificationRequest);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUser())
                    .ReturnsAsync(outputEntraUser);

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
                    .ProcessCsvIdentificationRequestAsync(inputCsvIdentificationRequestId);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId),
                    Times.Once);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    outputPersistanceOrchestrationAccessRequest.CsvIdentificationRequest),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUser(),
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

            identificationCoordinationServiceMock.VerifyNoOtherCalls();
            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
