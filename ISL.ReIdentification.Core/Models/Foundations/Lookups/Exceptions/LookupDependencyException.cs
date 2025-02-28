// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions
{
    public class LookupDependencyException : Xeption
    {
        public LookupDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}