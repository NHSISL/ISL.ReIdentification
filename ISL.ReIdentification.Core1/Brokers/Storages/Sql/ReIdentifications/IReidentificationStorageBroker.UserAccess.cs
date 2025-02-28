// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial interface IReIdentificationStorageBroker
    {
        ValueTask<UserAccess> InsertUserAccessAsync(UserAccess userAccess);
        ValueTask<IQueryable<UserAccess>> SelectAllUserAccessesAsync();
        ValueTask<UserAccess> SelectUserAccessByIdAsync(Guid userAccessId);
        ValueTask<UserAccess> UpdateUserAccessAsync(UserAccess userAccess);
        ValueTask<UserAccess> DeleteUserAccessAsync(UserAccess userAccess);
    }
}
