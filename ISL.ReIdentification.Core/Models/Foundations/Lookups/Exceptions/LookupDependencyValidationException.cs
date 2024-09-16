using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions
{
    public class LookupDependencyValidationException : Xeption
    {
        public LookupDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}