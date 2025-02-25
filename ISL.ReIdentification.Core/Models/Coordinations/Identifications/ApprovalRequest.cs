// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace ISL.ReIdentification.Core.Models.Coordinations.Identifications
{
    public class ApprovalRequest
    {
        public Guid ImpersonationContextId { get; set; }
        public bool IsApproved { get; set; }
    }
}
