﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace ISL.ReIdentification.Core.Models.Foundations.Audits
{
    public class Audit : IKey, IAudit
    {
        public Guid Id { get; set; }
        public Guid? CorrelationId { get; set; }
        public string AuditType { get; set; }
        public string AuditDetail { get; set; }
        public string LogLevel { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
