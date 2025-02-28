// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;

namespace ISL.ReIdentification.Core.Services.Processings.UserAccesses
{
    public interface IUserAccessProcessingService
    {
        ValueTask<UserAccess> AddUserAccessAsync(UserAccess userAccess);
        ValueTask<IQueryable<UserAccess>> RetrieveAllUserAccessesAsync();
        ValueTask<UserAccess> RetrieveUserAccessByIdAsync(Guid userAccessId);
        ValueTask<UserAccess> ModifyUserAccessAsync(UserAccess userAccess);
        ValueTask<UserAccess> RemoveUserAccessByIdAsync(Guid userAccessId);
        ValueTask<List<string>> RetrieveAllActiveOrganisationsUserHasAccessToAsync(string entraUserId);
        ValueTask BulkAddRemoveUserAccessAsync(BulkUserAccess bulkUserAccess);
    }
}
