﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.OdsDatas.Exceptions
{
    public class FailedServiceOdsDataException : Xeption
    {
        public FailedServiceOdsDataException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}