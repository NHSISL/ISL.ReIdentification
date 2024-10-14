// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.ImpersonationContexts
{
    public class ImpersonationContext
    {
        public Guid Id { get; set; }
        public Guid RequesterEntraUserId { get; set; } = Guid.Empty;
        public string RequesterFirstName { get; set; } = string.Empty;
        public string RequesterLastName { get; set; } = string.Empty;
        public string RequesterDisplayName { get; set; } = string.Empty;
        public string RequesterEmail { get; set; } = string.Empty;
        public string RequesterJobTitle { get; set; } = string.Empty;
        public Guid RecipientEntraUserId { get; set; } = Guid.Empty;
        public string RecipientFirstName { get; set; } = string.Empty;
        public string RecipientLastName { get; set; } = string.Empty;
        public string RecipientDisplayName { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
        public string RecipientJobTitle { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string Organisation { get; set; } = string.Empty;
        public string PipelineName { get; set; } = string.Empty;
        public string ManagedIdentityName { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public string IdentifierColumn { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
