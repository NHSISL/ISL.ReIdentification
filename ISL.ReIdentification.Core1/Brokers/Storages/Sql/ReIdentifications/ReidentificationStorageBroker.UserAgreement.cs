// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using Microsoft.EntityFrameworkCore;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker : EFxceptionsContext, IReIdentificationStorageBroker
    {
        public DbSet<UserAgreement> UserAgreements { get; set; }

        public async ValueTask<UserAgreement> InsertUserAgreementAsync(UserAgreement userAgreement) =>
            await InsertAsync(userAgreement);

        public async ValueTask<IQueryable<UserAgreement>> SelectAllUserAgreementsAsync() =>
            await SelectAllAsync<UserAgreement>();

        public async ValueTask<UserAgreement> SelectUserAgreementByIdAsync(Guid userAgreementId) =>
            await SelectAsync<UserAgreement>(userAgreementId);

        public async ValueTask<UserAgreement> UpdateUserAgreementAsync(UserAgreement userAgreement) =>
            await UpdateAsync(userAgreement);

        public async ValueTask<UserAgreement> DeleteUserAgreementAsync(UserAgreement userAgreement) =>
            await DeleteAsync(userAgreement);
    }
}