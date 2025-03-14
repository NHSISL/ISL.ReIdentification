﻿// ---------------------------------------------------------
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
                { "identifierColumnIndex", accessRequest.CsvIdentificationRequest.IdentifierColumnIndex },
                { "hasHeaderRecord" , accessRequest.CsvIdentificationRequest.HasHeaderRecord },
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
                { "identifierColumnIndex", accessRequest.CsvIdentificationRequest.IdentifierColumnIndex },
                { "hasHeaderRecord" , accessRequest.CsvIdentificationRequest.HasHeaderRecord },
                { "templateId", this.notificationConfigurations.CsvApprovedRequestTemplateId },
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
                { "identifierColumnIndex" , accessRequest.CsvIdentificationRequest.IdentifierColumnIndex },
                { "hasHeaderRecord" , accessRequest.CsvIdentificationRequest.HasHeaderRecord },
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
            string toEmail = accessRequest.ImpersonationContext.ResponsiblePersonEmail;
            string subject = "Pending Approval";
            string body = "Your request is pending approval";

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

            ValidateInputsOnSendImpersonationPendingApprovalNotificationAsync(toEmail, subject, body, personalisation);
            await this.notificationBroker.SendEmailAsync(toEmail, subject, body, personalisation);
        });

        public ValueTask SendImpersonationApprovedNotificationAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnSendImpersonationApprovedNotificationAsync(accessRequest, this.notificationConfigurations);
            string toEmail = accessRequest.ImpersonationContext.RequesterEmail;
            string subject = "Request Approved";
            string body = "Your request has been approved";

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
                { "templateId", this.notificationConfigurations.ImpersonationApprovedRequestTemplateId },
                { "configurationBaseUrl", this.notificationConfigurations.ConfigurationBaseUrl },
                { "portalBaseUrl", this.notificationConfigurations.PortalBaseUrl },
            };

            ValidateInputsOnSendImpersonationApprovedNotificationAsync(toEmail, subject, body, personalisation);
            await this.notificationBroker.SendEmailAsync(toEmail, subject, body, personalisation);
        });

        public ValueTask SendImpersonationDeniedNotificationAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnSendImpersonationDeniedNotificationAsync(accessRequest, this.notificationConfigurations);
            string toEmail = accessRequest.ImpersonationContext.RequesterEmail;
            string subject = "Request Denied";
            string body = "Your request has been denied";

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
                { "templateId", this.notificationConfigurations.ImpersonationDeniedRequestTemplateId },
                { "configurationBaseUrl", this.notificationConfigurations.ConfigurationBaseUrl },
                { "portalBaseUrl", this.notificationConfigurations.PortalBaseUrl },
            };

            ValidateInputsOnSendImpersonationDeniedNotificationAsync(toEmail, subject, body, personalisation);
            await this.notificationBroker.SendEmailAsync(toEmail, subject, body, personalisation);
        });

        public ValueTask SendImpersonationTokensGeneratedNotificationAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnSendImpersonationTokensGeneratedNotificationAsync(accessRequest, this.notificationConfigurations);
            string toEmail = accessRequest.ImpersonationContext.RequesterEmail;
            string subject = "Tokens Generated";
            string body = "SAS tokens have been generated";

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
                { "templateId", this.notificationConfigurations.ImpersonationTokensGeneratedTemplateId },
                { "configurationBaseUrl", this.notificationConfigurations.ConfigurationBaseUrl },
                { "portalBaseUrl", this.notificationConfigurations.PortalBaseUrl },
            };

            ValidateInputsOnSendImpersonationTokensGeneratedNotificationAsync(toEmail, subject, body, personalisation);
            await this.notificationBroker.SendEmailAsync(toEmail, subject, body, personalisation);
        });
    }
}
