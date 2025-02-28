// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions
{
    public class NullCsvIdentificationRequestException : Xeption
    {
        public NullCsvIdentificationRequestException(string message)
            : base(message)
        { }
    }
}
