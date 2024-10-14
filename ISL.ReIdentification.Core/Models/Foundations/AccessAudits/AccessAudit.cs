// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace ISL.ReIdentification.Core.Models.Foundations.AccessAudits
{

    public class AccessAudit : IKey, IAudit
    {
        public Guid Id { get; set; }
        public string PseudoIdentifier { get; set; }
        public string UserEmail { get; set; }
        public string Reason { get; set; }
        public bool HasAccess { get; set; }
        public string Message { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
