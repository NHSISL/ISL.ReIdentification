// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions
{
    public class InvalidArgumentPersistanceOrchestrationException : Xeption
    {
        public InvalidArgumentPersistanceOrchestrationException(string message)
            : base(message)
        { }
    }
}
