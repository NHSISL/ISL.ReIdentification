// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions
{
    public class IdentificationOrchestrationValidationException : Xeption
    {
        public IdentificationOrchestrationValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
