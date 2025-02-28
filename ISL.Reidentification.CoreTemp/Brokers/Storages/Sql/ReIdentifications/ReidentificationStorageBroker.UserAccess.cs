// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Microsoft.EntityFrameworkCore;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker : EFxceptionsContext, IReIdentificationStorageBroker
    {
        public DbSet<UserAccess> UserAccesses { get; set; }

        public async ValueTask<UserAccess> InsertUserAccessAsync(UserAccess userAccess) =>
            await InsertAsync(userAccess);

        public async ValueTask<IQueryable<UserAccess>> SelectAllUserAccessesAsync() =>
            await SelectAllAsync<UserAccess>();

        public async ValueTask<UserAccess> SelectUserAccessByIdAsync(Guid userAccessId) =>
            await SelectAsync<UserAccess>(userAccessId);

        public async ValueTask<UserAccess> UpdateUserAccessAsync(UserAccess userAccess) =>
            await UpdateAsync(userAccess);

        public async ValueTask<UserAccess> DeleteUserAccessAsync(UserAccess userAccess) =>
            await DeleteAsync(userAccess);
    }
}