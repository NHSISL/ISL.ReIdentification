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
        public async Task ShouldRetrieveImpersonationContextRequestAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputId = randomId;
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.IdentificationRequest = null;
            randomAccessRequest.CsvIdentificationRequest = null;
            AccessRequest outputAccessRequest = randomAccessRequest;
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            this.impersonationContextServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(inputId))
                    .ReturnsAsync(outputAccessRequest.ImpersonationContext);

            // when
            AccessRequest actualAccessRequest =
                await this.persistanceOrchestrationService
                    .RetrieveImpersonationContextByIdAsync(inputId);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.impersonationContextServiceMock.Verify(service =>
                service.RetrieveImpersonationContextByIdAsync(inputId),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.documentServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
