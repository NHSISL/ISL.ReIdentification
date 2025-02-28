// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions
{
    public class AccessAuditServiceException : Xeption
    {
        public AccessAuditServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
