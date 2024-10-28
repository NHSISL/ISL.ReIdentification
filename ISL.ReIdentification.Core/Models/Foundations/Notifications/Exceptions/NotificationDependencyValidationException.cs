// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Notifications.Exceptions
{
    public class NotificationDependencyValidationException : Xeption
    {
        public NotificationDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
