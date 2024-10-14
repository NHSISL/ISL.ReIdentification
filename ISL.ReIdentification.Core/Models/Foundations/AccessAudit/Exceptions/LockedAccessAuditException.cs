// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions
{
    public class LockedAccessAuditException : Xeption
    {
        public LockedAccessAuditException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
