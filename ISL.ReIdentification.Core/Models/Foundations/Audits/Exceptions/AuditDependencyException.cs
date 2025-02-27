﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Audits.Exceptions
{
    public class AuditDependencyException : Xeption
    {
        public AuditDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
