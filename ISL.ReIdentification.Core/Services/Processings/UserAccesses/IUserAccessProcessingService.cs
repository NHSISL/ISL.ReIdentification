// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAccesses
{
    public interface IUserAccessProcessingService
    {
        ValueTask<UserAccess> AddUserAccessAsync(UserAccess userAccess);
        ValueTask<IQueryable<UserAccess>> RetrieveAllUserAccessesAsync();
        ValueTask<UserAccess> RetrieveUserAccessByIdAsync(Guid userAccessId);
        ValueTask<UserAccess> ModifyUserAccessAsync(UserAccess userAccess);
        ValueTask<UserAccess> RemoveUserAccessByIdAsync(Guid userAccessId);
        ValueTask<List<string>> RetrieveAllActiveOrganisationsUserHasAccessTo(Guid entraUserId);
        ValueTask<BulkUserAccess> BulkAddRemoveUserAccessAsync(BulkUserAccess bulkUserAccess);
    }
}
