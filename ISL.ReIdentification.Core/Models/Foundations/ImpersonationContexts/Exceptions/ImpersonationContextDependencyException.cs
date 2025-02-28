// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions
{
    public class ImpersonationContextDependencyException : Xeption
    {
        public ImpersonationContextDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
