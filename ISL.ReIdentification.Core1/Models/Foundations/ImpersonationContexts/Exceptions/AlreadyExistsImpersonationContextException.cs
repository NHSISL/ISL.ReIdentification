// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions
{
    public class AlreadyExistsImpersonationContextException : Xeption
    {
        public AlreadyExistsImpersonationContextException(string message, Exception innerException, IDictionary data)
                : base(message, innerException, data)
        { }
    }
}
