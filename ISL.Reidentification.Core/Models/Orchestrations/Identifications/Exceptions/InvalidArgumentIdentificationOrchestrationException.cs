// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions
{
    public class InvalidArgumentIdentificationOrchestrationException : Xeption
    {
        public InvalidArgumentIdentificationOrchestrationException(string message)
            : base(message)
        { }
    }
}
