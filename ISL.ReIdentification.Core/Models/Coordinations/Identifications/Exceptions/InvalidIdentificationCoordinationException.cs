// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions
{
    public class InvalidIdentificationCoordinationException : Xeption
    {
        public InvalidIdentificationCoordinationException(string message)
            : base(message)
        { }
    }
}
