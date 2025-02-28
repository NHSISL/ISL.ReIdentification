// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions
{
    public class FailedStorageImpersonationContextException : Xeption
    {
        public FailedStorageImpersonationContextException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
