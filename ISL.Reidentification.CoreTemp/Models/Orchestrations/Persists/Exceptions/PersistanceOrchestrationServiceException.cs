// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions
{
    public class PersistanceOrchestrationServiceException : Xeption
    {
        public PersistanceOrchestrationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
