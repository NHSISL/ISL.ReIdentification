﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions
{
    public class PersistanceOrchestrationDependencyException : Xeption
    {
        public PersistanceOrchestrationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}