// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Audits.Exceptions
{
    public class AuditServiceException : Xeption
    {
        public AuditServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
