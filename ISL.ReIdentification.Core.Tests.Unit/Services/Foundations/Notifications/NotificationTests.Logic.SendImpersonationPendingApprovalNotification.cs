// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Notifications
{
    public partial class NotificationTests
    {
        [Fact]
        public async Task ShouldSendImpersonationPendingApprovalNotificationAsync()
        {
            // given
            AccessRequest accessRequest = CreateImpersonationContextAccessRequest();
            string toEmail = accessRequest.ImpersonationContext.ResponsiblePersonEmail;
            string subject = "Pending Approval";
            string body = "Your request is pending approval";
            string randomResponseId = GetRandomString();

            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
            {
                { "id", accessRequest.ImpersonationContext.Id.ToString() },
                { "requesterEntraUserId", accessRequest.ImpersonationContext.RequesterEntraUserId.ToString() },
                { "requesterFirstName", accessRequest.ImpersonationContext.RequesterFirstName },
                { "requesterLastName", accessRequest.ImpersonationContext.RequesterLastName },
                { "requesterDisplayName", accessRequest.ImpersonationContext.RequesterDisplayName },
                { "requesterEmail", accessRequest.ImpersonationContext.RequesterEmail },
                { "requesterJobTitle", accessRequest.ImpersonationContext.RequesterJobTitle },
                { "recipientEntraUserId", accessRequest.ImpersonationContext.ResponsiblePersonEntraUserId.ToString() },
                { "recipientFirstName", accessRequest.ImpersonationContext.ResponsiblePersonFirstName },
                { "recipientLastName", accessRequest.ImpersonationContext.ResponsiblePersonLastName },
                { "recipientDisplayName", accessRequest.ImpersonationContext.ResponsiblePersonDisplayName },
                { "recipientEmail", accessRequest.ImpersonationContext.ResponsiblePersonEmail },
                { "recipientJobTitle", accessRequest.ImpersonationContext.ResponsiblePersonJobTitle },
                { "reason", accessRequest.ImpersonationContext.Reason },
                { "organisation", accessRequest.ImpersonationContext.Organisation },
                { "identifierColumn", accessRequest.ImpersonationContext.IdentifierColumn },
                { "templateId", this.notificationConfigurations.ImpersonationPendingApprovalRequestTemplateId },
                { "configurationBaseUrl", this.notificationConfigurations.ConfigurationBaseUrl },
                { "portalBaseUrl", this.notificationConfigurations.PortalBaseUrl },
            };

            this.notificationBrokerMock.Setup(broker =>
                broker.SendEmailAsync(toEmail, subject, body, personalisation))
                    .ReturnsAsync(randomResponseId);

            // when
            await this.notificationService.SendImpersonationPendingApprovalNotificationAsync(accessRequest);

            // then
            this.notificationBrokerMock.Verify(broker =>
                broker.SendEmailAsync(toEmail, subject, body, It.Is(SameDictionaryAs(personalisation))),
                    Times.Once);

            this.notificationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
