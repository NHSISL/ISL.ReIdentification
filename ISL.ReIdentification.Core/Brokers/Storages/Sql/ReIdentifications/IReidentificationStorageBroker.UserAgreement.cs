// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial interface IReIdentificationStorageBroker
    {
        ValueTask<UserAgreement> InsertUserAgreementAsync(UserAgreement userAgreement);
        ValueTask<IQueryable<UserAgreement>> SelectAllUserAgreementsAsync();
        ValueTask<UserAgreement> SelectUserAgreementByIdAsync(Guid userAgreementId);
        ValueTask<UserAgreement> UpdateUserAgreementAsync(UserAgreement userAgreement);
        ValueTask<UserAgreement> DeleteUserAgreementAsync(UserAgreement userAgreement);
    }
}
