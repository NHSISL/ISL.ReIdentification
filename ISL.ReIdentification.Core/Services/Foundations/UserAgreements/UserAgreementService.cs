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

                return maybeUserAgreement;
            });
    }
}