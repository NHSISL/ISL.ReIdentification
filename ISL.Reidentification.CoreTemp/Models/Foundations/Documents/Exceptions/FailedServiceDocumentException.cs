// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions
{
    public class FailedServiceDocumentException : Xeption
    {
        public FailedServiceDocumentException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
