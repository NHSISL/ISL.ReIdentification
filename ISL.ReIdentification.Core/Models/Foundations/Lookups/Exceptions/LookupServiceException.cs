using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions
{
    public class LookupServiceException : Xeption
    {
        public LookupServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}