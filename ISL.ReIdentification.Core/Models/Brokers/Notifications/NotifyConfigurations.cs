// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace ISL.ReIdentification.Core.Models.Brokers.Notifications
{
    public class NotificationConfigurations
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ConfigurationBaseUrl { get; set; } = string.Empty;
        public string PortalBaseUrl { get; set; } = string.Empty;
        public string CsvPendingApprovalRequestTemplateId { get; set; } = string.Empty;
        public string CsvApprovedRequestTemplateId { get; set; } = string.Empty;
        public string CsvDeniedRequestTemplateId { get; set; } = string.Empty;
        public string ImpersonationPendingApprovalRequestTemplateId { get; set; } = string.Empty;
        public string ImpersonationApprovedRequestTemplateId { get; set; } = string.Empty;
        public string ImpersonationDeniedRequestTemplateId { get; set; } = string.Empty;
        public string ImpersonationTokensGeneratedTemplateId { get; set; } = string.Empty;
    }
}
