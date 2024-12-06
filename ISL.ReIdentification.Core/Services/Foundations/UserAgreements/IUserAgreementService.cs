// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAgreements
{
    public interface IUserAgreementService
    {
        ValueTask<UserAgreement> AddUserAgreementAsync(UserAgreement userAgreement);
        ValueTask<IQueryable<UserAgreement>> RetrieveAllUserAgreementsAsync();
        ValueTask<UserAgreement> RetrieveUserAgreementByIdAsync(Guid userAgreementId);
        ValueTask<UserAgreement> ModifyUserAgreementAsync(UserAgreement userAgreement);
        ValueTask<UserAgreement> RemoveUserAgreementByIdAsync(Guid userAgreementId);
    }
}