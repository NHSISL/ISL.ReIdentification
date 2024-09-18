﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions
{
    public class PdsDataServiceException : Xeption
    {
        public PdsDataServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
