﻿// ---------------------------------------------------------
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
        public async Task ShouldSendCsvApprovedNotificationAsync()
        {
            // given
            AccessRequest accessRequest = CreateRandomCsvAccessRequest();
            string toEmail = accessRequest.CsvIdentificationRequest.RequesterEmail;
            string subject = "Request Approved";
            string body = "Your request has been approved";
            string randomResponseId = GetRandomString();

            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
            {
                { "id", accessRequest.CsvIdentificationRequest.Id.ToString() },
                { "requesterEntraUserId", accessRequest.CsvIdentificationRequest.RequesterEntraUserId.ToString() },
                { "requesterFirstName", accessRequest.CsvIdentificationRequest.RequesterFirstName },
                { "requesterLastName", accessRequest.CsvIdentificationRequest.RequesterLastName },
                { "requesterDisplayName", accessRequest.CsvIdentificationRequest.RequesterDisplayName },
                { "requesterEmail", accessRequest.CsvIdentificationRequest.RequesterEmail },
                { "requesterJobTitle", accessRequest.CsvIdentificationRequest.RequesterJobTitle },
                { "recipientEntraUserId", accessRequest.CsvIdentificationRequest.RecipientEntraUserId.ToString() },
                { "recipientFirstName", accessRequest.CsvIdentificationRequest.RecipientFirstName },
                { "recipientLastName", accessRequest.CsvIdentificationRequest.RecipientLastName },
                { "recipientDisplayName", accessRequest.CsvIdentificationRequest.RecipientDisplayName },
                { "recipientEmail", accessRequest.CsvIdentificationRequest.RecipientEmail },
                { "recipientJobTitle", accessRequest.CsvIdentificationRequest.RecipientJobTitle },
                { "reason", accessRequest.CsvIdentificationRequest.Reason },
                { "organisation", accessRequest.CsvIdentificationRequest.Organisation },
                { "identifierColumnIndex" , accessRequest.CsvIdentificationRequest.IdentifierColumnIndex },
                { "hasHeaderRecord" , accessRequest.CsvIdentificationRequest.HasHeaderRecord },
                { "templateId", this.notificationConfigurations.CsvApprovedRequestTemplateId },
                { "configurationBaseUrl", this.notificationConfigurations.ConfigurationBaseUrl },
                { "portalBaseUrl", this.notificationConfigurations.PortalBaseUrl },
            };

            this.notificationBrokerMock.Setup(broker =>
                broker.SendEmailAsync(toEmail, subject, body, personalisation))
                    .ReturnsAsync(randomResponseId);

            // when
            await this.notificationService.SendCsvApprovedNotificationAsync(accessRequest);

            // then
            this.notificationBrokerMock.Verify(broker =>
                broker.SendEmailAsync(toEmail, subject, body, It.Is(SameDictionaryAs(personalisation))),
                    Times.Once);

            this.notificationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
