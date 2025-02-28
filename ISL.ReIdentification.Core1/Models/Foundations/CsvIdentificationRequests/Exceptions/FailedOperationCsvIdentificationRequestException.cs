// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions
{
    public class FailedOperationCsvIdentificationRequestException : Xeption
    {
        public FailedOperationCsvIdentificationRequestException(string message, DbUpdateException innerException)
            : base(message, innerException)
        { }
    }
}
