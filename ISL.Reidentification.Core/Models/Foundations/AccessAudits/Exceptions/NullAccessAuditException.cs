// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions
{
    public class NullAccessAuditException : Xeption
    {
        public NullAccessAuditException(string message)
            : base(message)
        { }
    }
}
