// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAgreements
{
    public partial class UserAgreementService : IUserAgreementService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public UserAgreementService(
            IReIdentificationStorageBroker reIdentificationStorageBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityBroker securityBroker,
            ILoggingBroker loggingBroker)
        {
            this.reIdentificationStorageBroker = reIdentificationStorageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityBroker = securityBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<UserAgreement> AddUserAgreementAsync(UserAgreement userAgreement) =>
        TryCatch(async () =>
        {
            UserAgreement userAgreementWithAddAuditApplied = await ApplyAddAuditAsync(userAgreement);
            await ValidateUserAgreementOnAddAsync(userAgreementWithAddAuditApplied);

            return await this.reIdentificationStorageBroker.InsertUserAgreementAsync(userAgreementWithAddAuditApplied);
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
                UserAgreement userAgreementWithModifyAuditApplied = await ApplyModifyAuditAsync(userAgreement);
                await ValidateUserAgreementOnModifyAsync(userAgreementWithModifyAuditApplied);

                UserAgreement maybeUserAgreement = await this.reIdentificationStorageBroker
                    .SelectUserAgreementByIdAsync(userAgreementWithModifyAuditApplied.Id);

                ValidateStorageUserAgreement(maybeUserAgreement, userAgreementWithModifyAuditApplied.Id);

                ValidateAgainstStorageUserAgreementOnModify(
                    inputUserAgreement: userAgreementWithModifyAuditApplied,
                    storageUserAgreement: maybeUserAgreement);

                return await this.reIdentificationStorageBroker.UpdateUserAgreementAsync(userAgreementWithModifyAuditApplied);
            });

        public ValueTask<UserAgreement> RemoveUserAgreementByIdAsync(Guid userAgreementId) =>
            TryCatch(async () =>
            {
                ValidateUserAgreementId(userAgreementId);

                UserAgreement maybeUserAgreement = await this.reIdentificationStorageBroker.SelectUserAgreementByIdAsync(userAgreementId);

                ValidateStorageUserAgreement(maybeUserAgreement, userAgreementId);

                UserAgreement userAgreementWithDeleteAuditApplied = await ApplyDeleteAuditAsync(maybeUserAgreement);

                UserAgreement updatedUserAgreement =
                    await this.reIdentificationStorageBroker.UpdateUserAgreementAsync(userAgreementWithDeleteAuditApplied);

                await ValidateAgainstStorageUserAgreementOnDeleteAsync(
                    updatedUserAgreement,
                    userAgreementWithDeleteAuditApplied);

                return await this.reIdentificationStorageBroker.DeleteUserAgreementAsync(updatedUserAgreement);
            });

        virtual internal async ValueTask<UserAgreement> ApplyAddAuditAsync(UserAgreement userAgreement)
        {
            ValidateUserAgreementIsNotNull(userAgreement);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            userAgreement.CreatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            userAgreement.CreatedDate = auditDateTimeOffset;
            userAgreement.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            userAgreement.UpdatedDate = auditDateTimeOffset;

            return userAgreement;
        }

        virtual internal async ValueTask<UserAgreement> ApplyModifyAuditAsync(UserAgreement userAgreement)
        {
            ValidateUserAgreementIsNotNull(userAgreement);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            userAgreement.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            userAgreement.UpdatedDate = auditDateTimeOffset;

            return userAgreement;
        }

        virtual internal async ValueTask<UserAgreement> ApplyDeleteAuditAsync(UserAgreement userAgreement)
        {
            ValidateUserAgreementIsNotNull(userAgreement);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            userAgreement.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            userAgreement.UpdatedDate = auditDateTimeOffset;
            return userAgreement;
        }
    }
}