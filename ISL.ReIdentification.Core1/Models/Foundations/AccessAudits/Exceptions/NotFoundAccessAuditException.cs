// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions
{
    public class NotFoundAccessAuditException : Xeption
    {
        public NotFoundAccessAuditException(string message)
            : base(message)
        { }
    }
}
