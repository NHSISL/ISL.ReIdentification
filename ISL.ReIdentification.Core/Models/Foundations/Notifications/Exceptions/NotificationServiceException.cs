// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions
{
    public class NotificationServiceException : Xeption
    {
        public NotificationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
