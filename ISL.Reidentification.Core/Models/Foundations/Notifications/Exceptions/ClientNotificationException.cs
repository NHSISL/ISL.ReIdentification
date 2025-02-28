﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Notifications.Exceptions
{
    public class ClientNotificationException : Xeption
    {
        public ClientNotificationException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
