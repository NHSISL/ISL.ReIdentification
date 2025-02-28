// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions
{
    public class CsvIdentificationRequestServiceException : Xeption
    {
        public CsvIdentificationRequestServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
