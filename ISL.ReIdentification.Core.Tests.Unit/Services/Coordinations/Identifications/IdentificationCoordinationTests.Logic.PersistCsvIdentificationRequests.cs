// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
            randomAccessRequest.IdentificationRequest = null;
            randomAccessRequest.ImpersonationContext = null;
            AccessRequest inputAccessRequest = randomAccessRequest;
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.PersistCsvIdentificationRequestAsync(inputAccessRequest))
                    .ReturnsAsync(outputAccessRequest);

            throw new NotImplementedException();
            // This needs to be a partial mock so we can vefify the virtual method call

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
