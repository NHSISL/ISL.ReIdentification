// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.ReIdentifications
{
    public class ApprovalRequest
    {
        public Guid ImpersonationContextId { get; set; }
        public bool IsApproved { get; set; }
    }
}
