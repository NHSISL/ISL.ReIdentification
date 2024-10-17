// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task ShouldProcessCsvIdentificationRequestAsync()
        {
            // given
            string randomCsvData = GetRandomString();
            string inputCsvData = randomCsvData;
            Guid randomCsvIdentificationRequestId = Guid.NewGuid();
            Guid inputCsvIdentificationRequestId = randomCsvIdentificationRequestId;
            AccessRequest randomPersistanceOrchestrationAccessRequest = CreateRandomAccessRequest();
            AccessRequest outputPersistanceOrchestrationAccessRequest = randomPersistanceOrchestrationAccessRequest;
            CsvIdentificationItem randomCsvIdentificationItem = CreateRandomCsvIdentificationItem();
            List<dynamic> outputCsvToObjectMapping = new List<dynamic> { randomCsvIdentificationItem };
            EntraUser randomEntraUser = CreateRandomEntraUser();
            EntraUser outputEntraUser = randomEntraUser;
            IdentificationRequest randomIdentificationRequest = CreateRandomIdentificationRequest();
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            AccessRequest createdAccessRequest = randomAccessRequest;
            createdAccessRequest.IdentificationRequest = randomIdentificationRequest;
            AccessRequest outputOrchestrationAccessRequest = CreateRandomAccessRequest();
            IdentificationRequest inputIdentificationRequest = createdAccessRequest.IdentificationRequest;
            IdentificationRequest outputOrchestrationIdentificationRequest = CreateRandomIdentificationRequest();
            string outputObjectToCsvMapping = GetRandomString();

            CsvIdentificationRequest createdCsvIdentificationRequest =
                CreateIdentificationRequestPopulatedCsvIdentificationRequest(outputOrchestrationIdentificationRequest,
                    outputObjectToCsvMapping);

            AccessRequest resultingAccessRequest = CreateRandomAccessRequest();
            resultingAccessRequest.CsvIdentificationRequest = createdCsvIdentificationRequest;
            resultingAccessRequest.IdentificationRequest = null;
            resultingAccessRequest.ImpersonationContext = null;
            AccessRequest expectedAccessRequest = resultingAccessRequest.DeepClone();

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId))
                    .ReturnsAsync(outputPersistanceOrchestrationAccessRequest);

            this.csvHelperBrokerMock.Setup(broker =>
               broker.MapCsvToObjectAsync<dynamic>(It.IsAny<string>(), true, null))
                   .ReturnsAsync(outputCsvToObjectMapping);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUser())
                    .ReturnsAsync(outputEntraUser);

            this.accessOrchestrationServiceMock.Setup(service =>
                service.ValidateAccessForIdentificationRequestAsync(It.IsAny<AccessRequest>()))
                    .ReturnsAsync(outputOrchestrationAccessRequest);

            this.identificationOrchestrationServiceMock.Setup(service =>
                service.ProcessIdentificationRequestAsync(It.IsAny<IdentificationRequest>()))
                    .ReturnsAsync(outputOrchestrationIdentificationRequest);

            this.csvHelperBrokerMock.Setup(broker =>
               broker.MapObjectToCsvAsync<CsvIdentificationItem>(
                   It.IsAny<List<CsvIdentificationItem>>(),
                   true,
                   null,
                   false))
                   .ReturnsAsync(outputObjectToCsvMapping);

            // when
            AccessRequest actualAccessRequest =
                await this.identificationCoordinationService
                    .ProcessCsvIdentificationRequestAsync(inputCsvIdentificationRequestId);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId),
                    Times.Once);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.MapCsvToObjectAsync<dynamic>(It.IsAny<string>(), true, null),
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

            this.csvHelperBrokerMock.Verify(broker =>
                broker.MapObjectToCsvAsync<CsvIdentificationItem>(
                    It.IsAny<List<CsvIdentificationItem>>(),
                    true,
                    null,
                    false),
                    Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
