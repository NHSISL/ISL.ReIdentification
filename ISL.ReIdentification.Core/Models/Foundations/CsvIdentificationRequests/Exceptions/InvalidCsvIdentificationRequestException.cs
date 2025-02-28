// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions
{
    public class InvalidCsvIdentificationRequestException : Xeption
    {
        public InvalidCsvIdentificationRequestException(string message)
            : base(message)
        { }
    }
}
