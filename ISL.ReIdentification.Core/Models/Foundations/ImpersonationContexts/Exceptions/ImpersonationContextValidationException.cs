// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions
{
    public class ImpersonationContextValidationException : Xeption
    {
        public ImpersonationContextValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
