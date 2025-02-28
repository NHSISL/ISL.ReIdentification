// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions
{
    public class FailedServiceImpersonationContextException : Xeption
    {
        public FailedServiceImpersonationContextException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
