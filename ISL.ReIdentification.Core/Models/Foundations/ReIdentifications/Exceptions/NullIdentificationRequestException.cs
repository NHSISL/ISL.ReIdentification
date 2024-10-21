// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.ReIdentifications.Exceptions
{
    public class NullIdentificationRequestException : Xeption
    {
        public NullIdentificationRequestException(string message)
            : base(message)
        { }
    }
}
