// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Audits.Exceptions
{
    public class FailedOperationAuditException : Xeption
    {
        public FailedOperationAuditException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
