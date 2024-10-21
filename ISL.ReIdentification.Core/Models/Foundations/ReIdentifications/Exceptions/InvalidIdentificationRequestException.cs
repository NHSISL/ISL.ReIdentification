// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.ReIdentifications.Exceptions
{
    public class InvalidIdentificationRequestException : Xeption
    {
        public InvalidIdentificationRequestException(string message)
            : base(message)
        { }
    }
}
