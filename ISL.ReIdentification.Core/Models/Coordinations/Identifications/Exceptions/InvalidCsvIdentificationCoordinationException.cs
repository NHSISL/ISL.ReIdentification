// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions
{
    public class InvalidCsvIdentificationCoordinationException : Xeption
    {
        public InvalidCsvIdentificationCoordinationException(string message)
            : base(message)
        { }
    }
}
