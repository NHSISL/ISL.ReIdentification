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
        public async Task ShouldProcessImpersonationContextRequestAsync()
        {
            // given
            AccessRequest inputAccessRequest = CreateRandomAccessRequest(); ;
            AccessRequest outputAccessOrchestrationAccessRequest = CreateRandomAccessRequest();
            outputAccessOrchestrationAccessRequest.CsvIdentificationRequest = null;
            outputAccessOrchestrationAccessRequest.IdentificationRequest = null;
            AccessRequest expectedAccessRequest = outputAccessOrchestrationAccessRequest.DeepClone();

            this.accessOrchestrationServiceMock.Setup(service =>
                service.ProcessImpersonationContextRequestAsync(inputAccessRequest))
                    .ReturnsAsync(outputAccessOrchestrationAccessRequest);

            // when
            AccessRequest actualAccessRequest =
                await identificationCoordinationService
                    .ProcessImpersonationContextRequestAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.accessOrchestrationServiceMock.Verify(service =>
                service.ProcessImpersonationContextRequestAsync(inputAccessRequest),
                    Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
