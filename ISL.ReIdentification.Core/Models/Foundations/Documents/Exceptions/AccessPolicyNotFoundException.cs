// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions
{
    public class AccessPolicyNotFoundException : Exception
    {
        public AccessPolicyNotFoundException(string message)
            : base(message)
        { }
    }

}
