// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions
{
    public class AlreadyExistsAccessAuditException : Xeption
    {
        public AlreadyExistsAccessAuditException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
