// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
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
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.projectStorageConfiguration)
            { CallBase = true };

            Guid inputCsvIdentificationRequestId = Guid.NewGuid();
            string inputReason = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset outputDateTimeOffset = randomDateTimeOffset;
            string outputTimestamp = outputDateTimeOffset.ToString("yyyyMMddHHmms");

            AccessRequest outputPersistanceOrchestrationAccessRequest =
                CreateRandomCsvIdentificationRequestAccessRequest();

            AccessRequest outputConversionAccessRequest = outputPersistanceOrchestrationAccessRequest.DeepClone();
            outputConversionAccessRequest.IdentificationRequest = CreateRandomIdentificationRequest();
            EntraUser outputEntraUser = CreateRandomEntraUser();
            AccessRequest outputOrchestrationAccessRequest = CreateRandomAccessRequest();
            IdentificationRequest outputOrchestrationIdentificationRequest = CreateRandomIdentificationRequest();
            AccessRequest inputAccessOrchestrationAccessRequest = outputConversionAccessRequest;
            AccessRequest inputIdentificationOrchestrationAccessRequest = outputOrchestrationAccessRequest.DeepClone();
            AccessRequest inputConversionAccessRequest = outputOrchestrationAccessRequest.DeepClone();
            inputConversionAccessRequest.IdentificationRequest = outputOrchestrationIdentificationRequest;
            AccessRequest resultingAccessRequest = CreateRandomCsvIdentificationRequestAccessRequest();
            AccessRequest expectedAccessRequest = resultingAccessRequest.DeepClone();
            string expectedFileName = $"{expectedAccessRequest.CsvIdentificationRequest.Filepath}_{outputTimestamp}.csv";
            expectedAccessRequest.CsvIdentificationRequest.Filepath = expectedFileName;

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId))
                    .ReturnsAsync(outputPersistanceOrchestrationAccessRequest);

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    It.Is(SameAccessRequestAs(outputPersistanceOrchestrationAccessRequest))))
                .ReturnsAsync(outputConversionAccessRequest);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(outputEntraUser);

            this.accessOrchestrationServiceMock.Setup(service =>
                service.ValidateAccessForIdentificationRequestAsync(
                    It.Is(SameAccessRequestAs(inputAccessOrchestrationAccessRequest))))
                        .ReturnsAsync(outputOrchestrationAccessRequest);

            this.identificationOrchestrationServiceMock.Setup(service =>
                service.ProcessIdentificationRequestAsync(
                        It.Is(SameIdentificationRequestAs(inputIdentificationOrchestrationAccessRequest.IdentificationRequest))))
                            .ReturnsAsync(outputOrchestrationIdentificationRequest);

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertIdentificationRequestToCsvIdentificationRequest(
                    It.Is(SameAccessRequestAs(inputConversionAccessRequest))))
                .ReturnsAsync(resultingAccessRequest);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(outputDateTimeOffset);

            IdentificationCoordinationService identificationCoordinationService =
                identificationCoordinationServiceMock.Object;

            // when
            AccessRequest actualAccessRequest =
                await identificationCoordinationService
                    .ProcessCsvIdentificationRequestAsync(inputCsvIdentificationRequestId, inputReason);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId),
                    Times.Once);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    It.Is(SameAccessRequestAs(outputPersistanceOrchestrationAccessRequest))),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.accessOrchestrationServiceMock.Verify(service =>
                service.ValidateAccessForIdentificationRequestAsync(
                    It.Is(SameAccessRequestAs(inputAccessOrchestrationAccessRequest))),
                        Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.ProcessIdentificationRequestAsync(
                        It.Is(SameIdentificationRequestAs(inputIdentificationOrchestrationAccessRequest.IdentificationRequest))),
                    Times.Once);

            identificationCoordinationServiceMock.Verify(service =>
                service.ConvertIdentificationRequestToCsvIdentificationRequest(
                    It.Is(SameAccessRequestAs(inputConversionAccessRequest))),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
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
