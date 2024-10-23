// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveCsvIdentificationRequestAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputId = randomId;
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.IdentificationRequest = null;
            randomAccessRequest.ImpersonationContext = null;
            AccessRequest outputAccessRequest = randomAccessRequest;
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(inputId))
                    .ReturnsAsync(outputAccessRequest.CsvIdentificationRequest);

            // when
            AccessRequest actualAccessRequest =
                await this.persistanceOrchestrationService
                    .RetrieveCsvIdentificationRequestByIdAsync(inputId);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(inputId),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
        }
    }
}
