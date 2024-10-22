// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldStoreRequestAndEmailWhenPersistNewImpersonationContextAsync()
        {
            // given
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.CsvIdentificationRequest = null;
            randomAccessRequest.IdentificationRequest = null;
            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            ImpersonationContext outputImpersonationContext = CreateRandomImpersonationContext();
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            outputAccessRequest.ImpersonationContext = outputImpersonationContext;
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            IQueryable<ImpersonationContext> emptyRetrieveAllImpersonationContexts =
                Enumerable.Empty<ImpersonationContext>().AsQueryable();

            this.impersonationContextServiceMock.Setup(service =>
                service.RetrieveAllImpersonationContextsAsync())
                    .ReturnsAsync(emptyRetrieveAllImpersonationContexts);

            this.impersonationContextServiceMock.Setup(service =>
                service.AddImpersonationContextAsync(inputAccessRequest.ImpersonationContext))
                    .ReturnsAsync(outputImpersonationContext);

            // when
            AccessRequest actualAccessRequest =
                await this.persistanceOrchestrationService
                    .PersistImpersonationContextAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.impersonationContextServiceMock.Verify(service =>
                service.RetrieveAllImpersonationContextsAsync(),
                    Times.Once);

            this.impersonationContextServiceMock.Verify(service =>
                service.AddImpersonationContextAsync(inputAccessRequest.ImpersonationContext),
                    Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendPendingApprovalNotificationAsync(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldUpdateRequestAndEmailWhenPersistApprovedImpersonationContextAsync()
        {
            // given
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.CsvIdentificationRequest = null;
            randomAccessRequest.IdentificationRequest = null;
            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            inputAccessRequest.ImpersonationContext.IsApproved = true;
            ImpersonationContext returnedImpersonationContext = inputAccessRequest.ImpersonationContext;
            returnedImpersonationContext.IsApproved = false;
            ImpersonationContext updatedImpersonationContext = returnedImpersonationContext.DeepClone();
            updatedImpersonationContext.IsApproved = true;
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            outputAccessRequest.ImpersonationContext = updatedImpersonationContext;
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            IQueryable<ImpersonationContext> randomImpersonationContexts =
                new List<ImpersonationContext> { returnedImpersonationContext }.AsQueryable();

            this.impersonationContextServiceMock.Setup(service =>
                service.RetrieveAllImpersonationContextsAsync())
                    .ReturnsAsync(randomImpersonationContexts);

            this.impersonationContextServiceMock.Setup(service =>
                service.ModifyImpersonationContextAsync(updatedImpersonationContext))
                    .ReturnsAsync(updatedImpersonationContext);

            // when
            AccessRequest actualAccessRequest =
                await this.persistanceOrchestrationService
                    .PersistImpersonationContextAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.impersonationContextServiceMock.Verify(service =>
                service.RetrieveAllImpersonationContextsAsync(),
                    Times.Once);

            this.impersonationContextServiceMock.Verify(service =>
                service.ModifyImpersonationContextAsync(updatedImpersonationContext),
                    Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendApprovedNotificationAsync(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }
    }
}
