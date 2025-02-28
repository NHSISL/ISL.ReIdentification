﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions
{
    public class AccessOrchestrationServiceException : Xeption
    {
        public AccessOrchestrationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
