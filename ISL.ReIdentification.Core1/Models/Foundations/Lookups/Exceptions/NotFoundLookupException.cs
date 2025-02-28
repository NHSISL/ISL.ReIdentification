// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions
{
    public class NotFoundLookupException : Xeption
    {
        public NotFoundLookupException(string message)
            : base(message)
        { }
    }
}