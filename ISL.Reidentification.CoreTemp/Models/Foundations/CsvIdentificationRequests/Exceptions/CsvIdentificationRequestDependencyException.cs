// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions
{
    public class CsvIdentificationRequestDependencyException : Xeption
    {
        public CsvIdentificationRequestDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
