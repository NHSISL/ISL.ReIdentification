// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions
{
    public class InvalidAccessIdentificationCoordinationException : Xeption
    {
        public InvalidAccessIdentificationCoordinationException(string message)
            : base(message)
        { }
    }
}
