using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions
{
    public class InvalidLookupException : Xeption
    {
        public InvalidLookupException(string message)
            : base(message)
        { }
    }
}