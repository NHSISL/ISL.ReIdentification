// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions
{
    public class CsvIdentificationRequestValidationException : Xeption
    {
        public CsvIdentificationRequestValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
