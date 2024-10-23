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
                service.SendImpersonationPendingApprovalNotificationAsync(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
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
            ImpersonationContext returnedImpersonationContext = inputAccessRequest.ImpersonationContext.DeepClone();
            returnedImpersonationContext.IsApproved = false;
            ImpersonationContext outputImpersonationContext = returnedImpersonationContext.DeepClone();
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            outputAccessRequest.ImpersonationContext = outputImpersonationContext;
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            IQueryable<ImpersonationContext> randomImpersonationContexts =
                new List<ImpersonationContext> { returnedImpersonationContext }.AsQueryable();

            this.impersonationContextServiceMock.Setup(service =>
                service.RetrieveAllImpersonationContextsAsync())
                    .ReturnsAsync(randomImpersonationContexts);

            this.impersonationContextServiceMock.Setup(service =>
                service.ModifyImpersonationContextAsync(inputAccessRequest.ImpersonationContext))
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
                service.ModifyImpersonationContextAsync(inputAccessRequest.ImpersonationContext),
                    Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendImpersonationApprovedNotificationAsync(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldUpdateRequestAndEmailWhenPersistDeniedImpersonationContextAsync()
        {
            // given
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.CsvIdentificationRequest = null;
            randomAccessRequest.IdentificationRequest = null;
            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            inputAccessRequest.ImpersonationContext.IsApproved = false;
            ImpersonationContext returnedImpersonationContext = inputAccessRequest.ImpersonationContext.DeepClone();
            returnedImpersonationContext.IsApproved = true;
            ImpersonationContext outputImpersonationContext = returnedImpersonationContext.DeepClone();
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            outputAccessRequest.ImpersonationContext = outputImpersonationContext;
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            IQueryable<ImpersonationContext> randomImpersonationContexts =
                new List<ImpersonationContext> { returnedImpersonationContext }.AsQueryable();

            this.impersonationContextServiceMock.Setup(service =>
                service.RetrieveAllImpersonationContextsAsync())
                    .ReturnsAsync(randomImpersonationContexts);

            this.impersonationContextServiceMock.Setup(service =>
                service.ModifyImpersonationContextAsync(inputAccessRequest.ImpersonationContext))
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
                service.ModifyImpersonationContextAsync(inputAccessRequest.ImpersonationContext),
                    Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendImpersonationDeniedNotificationAsync(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldNotUpdateRequestWhenPersistUnchangedImpersonationContextAsync()
        {
            // given
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.CsvIdentificationRequest = null;
            randomAccessRequest.IdentificationRequest = null;
            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            ImpersonationContext returnedImpersonationContext = inputAccessRequest.ImpersonationContext.DeepClone();
            ImpersonationContext outputImpersonationContext = returnedImpersonationContext.DeepClone();
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            outputAccessRequest.ImpersonationContext = outputImpersonationContext;
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            IQueryable<ImpersonationContext> randomImpersonationContexts =
                new List<ImpersonationContext> { returnedImpersonationContext }.AsQueryable();

            this.impersonationContextServiceMock.Setup(service =>
                service.RetrieveAllImpersonationContextsAsync())
                    .ReturnsAsync(randomImpersonationContexts);

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
                service.ModifyImpersonationContextAsync(inputAccessRequest.ImpersonationContext),
                    Times.Never);

            this.notificationServiceMock.Verify(service =>
                service.SendImpersonationPendingApprovalNotificationAsync(It.IsAny<AccessRequest>()),

                    Times.Never);

            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
        }
    }
}
