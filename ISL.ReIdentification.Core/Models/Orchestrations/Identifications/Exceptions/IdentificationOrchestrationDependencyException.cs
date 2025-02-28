// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions
{
    public class IdentificationOrchestrationDependencyException : Xeption
    {
        public IdentificationOrchestrationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
