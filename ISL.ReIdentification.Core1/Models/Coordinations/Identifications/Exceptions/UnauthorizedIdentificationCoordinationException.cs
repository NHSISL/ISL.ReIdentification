// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions
{
    public class UnauthorizedIdentificationCoordinationException : Xeption
    {
        public UnauthorizedIdentificationCoordinationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
