// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAccesses
{
    public partial class UserAccessProcessingService
    {
        private async ValueTask ValidateOnAddUserAccessAsync(UserAccess userAccess)
        {
            ValidateUserAccessIsNotNull(userAccess);
        }

        private static void ValidateUserAccessIsNotNull(UserAccess userAccess)
        {
            if (userAccess is null)
            {
                throw new NullUserAccessProcessingException("User access is null.");
            }
        }
    }
}
