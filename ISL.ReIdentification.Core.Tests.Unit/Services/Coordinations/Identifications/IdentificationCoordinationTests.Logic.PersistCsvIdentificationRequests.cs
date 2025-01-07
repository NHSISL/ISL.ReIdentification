// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task ShouldPersistCsvIdentificationRequestAsync()
        {
            // given
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.IdentificationRequest = null;
            randomAccessRequest.ImpersonationContext = null;
            AccessRequest inputAccessRequest = randomAccessRequest;
            AccessRequest postConversionAccessRequest = inputAccessRequest.DeepClone();
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

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

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.PersistCsvIdentificationRequestAsync(inputAccessRequest))
                    .ReturnsAsync(outputAccessRequest);

            identificationCoordinationServiceMock.Setup(service =>
                service.ConvertCsvIdentificationRequestToIdentificationRequest(
                    It.Is(SameAccessRequestAs(inputAccessRequest))))
                        .ReturnsAsync(postConversionAccessRequest);

            IdentificationCoordinationService identificationCoordinationService =
                identificationCoordinationServiceMock.Object;

            // when
            AccessRequest actualAccessRequest = await identificationCoordinationService
                .PersistsCsvIdentificationRequestAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.PersistCsvIdentificationRequestAsync(inputAccessRequest),
                    Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
