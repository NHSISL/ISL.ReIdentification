// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions
{
    public class IdentificationCoordinationDependencyException : Xeption
    {
        public IdentificationCoordinationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
