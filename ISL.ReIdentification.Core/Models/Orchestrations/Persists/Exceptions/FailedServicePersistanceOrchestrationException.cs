// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions
{
    public class FailedServicePersistanceOrchestrationException : Xeption
    {
        public FailedServicePersistanceOrchestrationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
