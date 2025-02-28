// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions
{
    public class PdsDataDependencyValidationException : Xeption
    {
        public PdsDataDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}