// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions
{
    public class InvalidAccessAuditException : Xeption
    {
        public InvalidAccessAuditException(string message)
            : base(message)
        { }
    }
}
