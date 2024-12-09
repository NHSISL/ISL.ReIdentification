// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions
{
    public class NullCsvReIdentificationConfigurationException : Xeption
    {
        public NullCsvReIdentificationConfigurationException(string message)
            : base(message)
        {
        }
    }
}
