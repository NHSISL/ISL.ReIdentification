// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.OdsDatas.Exceptions
{
    public class NotFoundOdsDataException : Xeption
    {
        public NotFoundOdsDataException(string message)
            : base(message)
        { }
    }
}