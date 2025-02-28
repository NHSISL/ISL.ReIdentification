// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions
{
    public class InvalidImpersonationContextException : Xeption
    {
        public InvalidImpersonationContextException(string message)
            : base(message)
        { }
    }
}
