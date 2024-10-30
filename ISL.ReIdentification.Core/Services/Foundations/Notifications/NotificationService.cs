// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;

namespace ISL.ReIdentification.Core.Services.Foundations.Notifications
{
    public partial class NotificationService : INotificationService
    {
        private readonly NotificationConfigurations notificationConfigurations;
        private readonly INotificationBroker notificationBroker;
        private readonly ILoggingBroker loggingBroker;

        public NotificationService(
            NotificationConfigurations notificationConfigurations,
            INotificationBroker notificationBroker,
            ILoggingBroker loggingBroker)
        {
            this.notificationConfigurations = notificationConfigurations;
            this.notificationBroker = notificationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask SendCsvPendingApprovalNotificationAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnSendCsvPendingApprovalNotificationAsync(accessRequest, this.notificationConfigurations);

            string toEmail = accessRequest.CsvIdentificationRequest.RecipientEmail;
            string subject = "Pending Approval";
            string body = "Your request is pending approval";

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
                { "identifierColumn", accessRequest.CsvIdentificationRequest.IdentifierColumn },
                { "templateId", this.notificationConfigurations.CsvPendingApprovalRequestTemplateId },
                { "configurationBaseUrl", this.notificationConfigurations.ConfigurationBaseUrl },
                { "portalBaseUrl", this.notificationConfigurations.PortalBaseUrl },
            };

            ValidateInputsOnSendCsvPendingApprovalNotificationAsync(toEmail, subject, body, personalisation);

            await this.notificationBroker.SendEmailAsync(toEmail, subject, body, personalisation);
        });

        public ValueTask SendCsvApprovedNotificationAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnSendCsvApprovedNotificationAsync(accessRequest, this.notificationConfigurations);

            string toEmail = accessRequest.CsvIdentificationRequest.RequesterEmail;
            string subject = "Request Approved";
            string body = "Your request has been approved";

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
                { "identifierColumn", accessRequest.CsvIdentificationRequest.IdentifierColumn },
                { "templateId", this.notificationConfigurations.CsvPendingApprovalRequestTemplateId },
                { "configurationBaseUrl", this.notificationConfigurations.ConfigurationBaseUrl },
                { "portalBaseUrl", this.notificationConfigurations.PortalBaseUrl },
            };

            ValidateInputsOnSendCsvApprovedNotificationAsync(toEmail, subject, body, personalisation);

            await this.notificationBroker.SendEmailAsync(toEmail, subject, body, personalisation);
        });

        public ValueTask SendCsvDeniedNotificationAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnSendCsvDeniedNotificationAsync(accessRequest, this.notificationConfigurations);

            string toEmail = accessRequest.CsvIdentificationRequest.RequesterEmail;
            string subject = "Request Denied";
            string body = "Your request has been denied";

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
                { "identifierColumn", accessRequest.CsvIdentificationRequest.IdentifierColumn },
                { "templateId", this.notificationConfigurations.CsvPendingApprovalRequestTemplateId },
                { "configurationBaseUrl", this.notificationConfigurations.ConfigurationBaseUrl },
                { "portalBaseUrl", this.notificationConfigurations.PortalBaseUrl },
            };

            ValidateInputsOnSendCsvDeniedNotificationAsync(toEmail, subject, body, personalisation);

            await this.notificationBroker.SendEmailAsync(toEmail, subject, body, personalisation);
        });

        public ValueTask SendImpersonationPendingApprovalNotificationAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnSendImpersonationPendingApprovalNotificationAsync(accessRequest, this.notificationConfigurations);

            string toEmail = accessRequest.CsvIdentificationRequest.RecipientEmail;
            string subject = "Pending Approval";
            string body = "Your request is pending approval";

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
                { "identifierColumn", accessRequest.CsvIdentificationRequest.IdentifierColumn },
                { "templateId", this.notificationConfigurations.ImpersonationPendingApprovalRequestTemplateId },
                { "configurationBaseUrl", this.notificationConfigurations.ConfigurationBaseUrl },
                { "portalBaseUrl", this.notificationConfigurations.PortalBaseUrl },
            };

            ValidateInputsOnSendImpersonationPendingApprovalNotificationAsync(toEmail, subject, body, personalisation);

            await this.notificationBroker.SendEmailAsync(toEmail, subject, body, personalisation);
        });

        public ValueTask SendImpersonationApprovedNotificationAsync(AccessRequest accessRequest) =>
            throw new System.NotImplementedException();

        public ValueTask SendImpersonationDeniedNotificationAsync(AccessRequest accessRequest) =>
            throw new System.NotImplementedException();
    }
}
