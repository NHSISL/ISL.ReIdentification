// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAgreements
{
    public partial class UserAgreementService : IUserAgreementService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public UserAgreementService(
            IReIdentificationStorageBroker reIdentificationStorageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.reIdentificationStorageBroker = reIdentificationStorageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<UserAgreement> AddUserAgreementAsync(UserAgreement userAgreement) =>
            TryCatch(async () =>
            {
                await ValidateUserAgreementOnAddAsync(userAgreement);

                return await this.reIdentificationStorageBroker.InsertUserAgreementAsync(userAgreement);
            });

        public ValueTask<IQueryable<UserAgreement>> RetrieveAllUserAgreementsAsync() =>
            TryCatch(async () => await this.reIdentificationStorageBroker.SelectAllUserAgreementsAsync());

        public ValueTask<UserAgreement> RetrieveUserAgreementByIdAsync(Guid userAgreementId) =>
            TryCatch(async () =>
            {
                ValidateUserAgreementId(userAgreementId);

                UserAgreement maybeUserAgreement = await this.reIdentificationStorageBroker
                    .SelectUserAgreementByIdAsync(userAgreementId);

                ValidateStorageUserAgreement(maybeUserAgreement, userAgreementId);

                return maybeUserAgreement;
            });

        public ValueTask<UserAgreement> ModifyUserAgreementAsync(UserAgreement userAgreement) =>
            TryCatch(async () =>
            {
                await ValidateUserAgreementOnModifyAsync(userAgreement);

                UserAgreement maybeUserAgreement =
                    await this.reIdentificationStorageBroker.SelectUserAgreementByIdAsync(userAgreement.Id);

                ValidateStorageUserAgreement(maybeUserAgreement, userAgreement.Id);
                ValidateAgainstStorageUserAgreementOnModify(inputUserAgreement: userAgreement, storageUserAgreement: maybeUserAgreement);

                return await this.reIdentificationStorageBroker.UpdateUserAgreementAsync(userAgreement);
            });

        public ValueTask<UserAgreement> RemoveUserAgreementByIdAsync(Guid userAgreementId) =>
            TryCatch(async () =>
            {
                ValidateUserAgreementId(userAgreementId);

                UserAgreement maybeUserAgreement = await this.reIdentificationStorageBroker
                    .SelectUserAgreementByIdAsync(userAgreementId);

                ValidateStorageUserAgreement(maybeUserAgreement, userAgreementId);

                return await this.reIdentificationStorageBroker.DeleteUserAgreementAsync(maybeUserAgreement);
            });
    }
}