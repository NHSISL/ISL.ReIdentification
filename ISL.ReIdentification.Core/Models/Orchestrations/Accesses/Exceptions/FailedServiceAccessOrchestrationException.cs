// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions
{
    public class FailedServiceAccessOrchestrationException : Xeption
    {
        public FailedServiceAccessOrchestrationException(string message, Exception innerException)
            : base(message)
        { }
    }
}
