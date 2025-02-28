// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions
{
    public class ImpersonationContextServiceException : Xeption
    {
        public ImpersonationContextServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
