// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions
{
    public class FailedServiceIdentificationOrchestrationException : Xeption
    {
        public FailedServiceIdentificationOrchestrationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
