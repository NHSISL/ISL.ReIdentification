// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions
{
    public class NullPdsDataException : Xeption
    {
        public NullPdsDataException(string message)
            : base(message)
        { }
    }
}