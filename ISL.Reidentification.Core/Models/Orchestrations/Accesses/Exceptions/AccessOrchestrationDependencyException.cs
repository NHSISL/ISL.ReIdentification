// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions
{
    public class AccessOrchestrationDependencyException : Xeption
    {
        public AccessOrchestrationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
