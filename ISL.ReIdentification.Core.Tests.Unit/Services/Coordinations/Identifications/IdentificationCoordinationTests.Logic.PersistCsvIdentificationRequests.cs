// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
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
            AccessRequest inputAccessRequest = randomAccessRequest;
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.PersistCsvIdentificationRequestAsync(inputAccessRequest))
                    .ReturnsAsync(outputAccessRequest);

            // when
            AccessRequest actualAccessRequest = await this.identificationCoordinationService
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
        }
    }
}
