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
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.projectStorageConfiguration)
            { CallBase = true };

            Guid inputCsvIdentificationRequestId = Guid.NewGuid();
            string inputReason = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset outputDateTimeOffset = randomDateTimeOffset;
            string outputTimestamp = outputDateTimeOffset.ToString("yyyyMMddHHmms");
            AccessRequest outputPersistanceOrchestrationAccessRequest = CreateRandomAccessRequest(); ;
            IdentificationRequest outputConversionIdentificationRequest = CreateRandomIdentificationRequest();
            EntraUser outputEntraUser = CreateRandomEntraUser();
            AccessRequest outputOrchestrationAccessRequest = CreateRandomAccessRequest();
            IdentificationRequest outputOrchestrationIdentificationRequest = CreateRandomIdentificationRequest();

            AccessRequest inputAccessOrchestrationAccessRequest = new AccessRequest
            {
                IdentificationRequest = outputConversionIdentificationRequest,
            };

            CsvIdentificationRequest outputConversionCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            AccessRequest resultingAccessRequest = CreateRandomAccessRequest();
            resultingAccessRequest.CsvIdentificationRequest = outputConversionCsvIdentificationRequest;
            resultingAccessRequest.IdentificationRequest = null;
            resultingAccessRequest.ImpersonationContext = null;
            AccessRequest expectedAccessRequest = resultingAccessRequest.DeepClone();
            string expectedFileName = $"data_{outputTimestamp}.csv";
            expectedAccessRequest.CsvIdentificationRequest.Filepath = expectedFileName;

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
                    outputPersistanceOrchestrationAccessRequest.CsvIdentificationRequest),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUser(),
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
