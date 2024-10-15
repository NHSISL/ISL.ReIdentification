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
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task ShouldProcessCsvIdentificationRequestAsync()
        {
            // given
            Guid randomCsvIdentificationRequestId = Guid.NewGuid();
            Guid inputCsvIdentificationRequestId = randomCsvIdentificationRequestId;
            AccessRequest randomPersistanceOrchestrationAccessRequest = CreateRandomAccessRequest();
            AccessRequest outputPersistanceOrchestrationAccessRequest = randomPersistanceOrchestrationAccessRequest;
            IdentificationRequest randomIdentificationRequest = CreateRandomIdentificationRequest();
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            AccessRequest createdAccessRequest = randomAccessRequest;
            createdAccessRequest.IdentificationRequest = randomIdentificationRequest;
            AccessRequest outputOrchestrationAccessRequest = CreateRandomAccessRequest();
            IdentificationRequest inputIdentificationRequest = createdAccessRequest.IdentificationRequest;
            IdentificationRequest outputIdentificationRequest = CreateRandomIdentificationRequest();
            CsvIdentificationRequest createdCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            AccessRequest resultingAccessRequest = CreateRandomAccessRequest();
            resultingAccessRequest.CsvIdentificationRequest = createdCsvIdentificationRequest;
            AccessRequest expectedAccessRequest = resultingAccessRequest.DeepClone();

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestAsync(inputCsvIdentificationRequestId))
                    .ReturnsAsync(outputPersistanceOrchestrationAccessRequest);

            this.accessOrchestrationServiceMock.Setup(service =>
                service.ValidateAccessForIdentificationRequestAsync(createdAccessRequest))
                    .ReturnsAsync(outputOrchestrationAccessRequest);

            this.identificationOrchestrationServiceMock.Setup(service =>
                service.ProcessIdentificationRequestAsync(outputOrchestrationAccessRequest.IdentificationRequest))
                    .ReturnsAsync(outputIdentificationRequest);

            // when
            AccessRequest actualAccessRequest =
                await this.identificationCoordinationService
                    .ProcessCsvIdentificationRequestAsync(inputCsvIdentificationRequestId);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestAsync(inputCsvIdentificationRequestId),
                    Times.Once);

            this.accessOrchestrationServiceMock.Verify(service =>
                service.ValidateAccessForIdentificationRequestAsync(createdAccessRequest),
                    Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.ProcessIdentificationRequestAsync(outputOrchestrationAccessRequest.IdentificationRequest),
                    Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
