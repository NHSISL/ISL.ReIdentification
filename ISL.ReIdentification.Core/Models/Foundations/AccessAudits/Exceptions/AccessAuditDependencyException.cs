// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions
{
    public class AccessAuditDependencyException : Xeption
    {
        public AccessAuditDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
