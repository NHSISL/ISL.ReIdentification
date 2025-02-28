// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions
{
    public class LockedImpersonationContextException : Xeption
    {
        public LockedImpersonationContextException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
