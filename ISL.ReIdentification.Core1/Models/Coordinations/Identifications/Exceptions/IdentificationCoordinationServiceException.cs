// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions
{
    public class IdentificationCoordinationServiceException : Xeption
    {
        public IdentificationCoordinationServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
