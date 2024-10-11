// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldStoreImpersonationContextRequestAndSendApprovalEmailWhenProcessingNewImpersonationContextRequest()
        {
            // given
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            string recipientEmail = inputAccessRequest.ImpersonationContextRequest.RecipientEmail;
            string randomSubject = GetRandomString();
            string randomBody = GetRandomString();
            Dictionary<string, dynamic> randomPersonalisation = new Dictionary<string, dynamic>();
            string randomIdentifier = GetRandomString();
            AccessRequest expectedAccessRequest = inputAccessRequest.DeepClone();
            expectedAccessRequest.ImpersonationContextRequest.CreatedDate = DateTimeOffset.UtcNow;
            expectedAccessRequest.ImpersonationContextRequest.IsApproved = null;

            IQueryable<ImpersonationContext> impersonationContextes =
                new List<ImpersonationContext> { randomImpersonationContext }.AsQueryable();

            this.impersonationContextServiceMock.Setup(service =>
                service.RetrieveAllImpersonationContextsAsync())
                .ReturnsAsync(impersonationContextes);

            this.impersonationContextServiceMock.Setup(service =>
                service.AddImpersonationContextAsync(inputAccessRequest.ImpersonationContextRequest))
                .ReturnsAsync(expectedAccessRequest.ImpersonationContextRequest);

            this.notificationBrokerMock.Setup(broker =>
                broker.SendEmailAsync(recipientEmail, randomSubject, randomBody, randomPersonalisation))
                .ReturnsAsync(randomIdentifier);

            // when
            AccessRequest actualAccessRequest =
                await this.accessOrchestrationService.ProcessImpersonationContextRequestAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.impersonationContextServiceMock.Verify(service =>
                service.RetrieveAllImpersonationContextsAsync(),
                Times.Once);

            this.notificationBrokerMock.Verify(broker =>
                broker.SendEmailAsync(recipientEmail, randomSubject, randomBody, randomPersonalisation),
                Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.notificationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
