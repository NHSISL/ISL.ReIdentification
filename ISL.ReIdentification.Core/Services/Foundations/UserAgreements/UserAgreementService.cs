using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Sql;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAgreements
{
    public partial class UserAgreementService : IUserAgreementService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public UserAgreementService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<UserAgreement> AddUserAgreementAsync(UserAgreement userAgreement) =>
            TryCatch(async () =>
            {
                ValidateUserAgreementOnAdd(userAgreement);

                return await this.storageBroker.InsertUserAgreementAsync(userAgreement);
            });

        public IQueryable<UserAgreement> RetrieveAllUserAgreements() =>
            TryCatch(() => this.storageBroker.SelectAllUserAgreements());

        public ValueTask<UserAgreement> RetrieveUserAgreementByIdAsync(Guid userAgreementId) =>
            TryCatch(async () =>
            {
                ValidateUserAgreementId(userAgreementId);

                UserAgreement maybeUserAgreement = await this.storageBroker
                    .SelectUserAgreementByIdAsync(userAgreementId);

                ValidateStorageUserAgreement(maybeUserAgreement, userAgreementId);

                return maybeUserAgreement;
            });

        public ValueTask<UserAgreement> ModifyUserAgreementAsync(UserAgreement userAgreement) =>
            TryCatch(async () =>
            {
                ValidateUserAgreementOnModify(userAgreement);

                UserAgreement maybeUserAgreement =
                    await this.storageBroker.SelectUserAgreementByIdAsync(userAgreement.Id);

                ValidateStorageUserAgreement(maybeUserAgreement, userAgreement.Id);
                ValidateAgainstStorageUserAgreementOnModify(inputUserAgreement: userAgreement, storageUserAgreement: maybeUserAgreement);

                return await this.storageBroker.UpdateUserAgreementAsync(userAgreement);
            });

        public async ValueTask<UserAgreement> RemoveUserAgreementByIdAsync(Guid userAgreementId)
        {
            UserAgreement maybeUserAgreement = await this.storageBroker
                    .SelectUserAgreementByIdAsync(userAgreementId);

            return await this.storageBroker.DeleteUserAgreementAsync(maybeUserAgreement);
        }
    }
}