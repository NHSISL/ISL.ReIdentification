﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAccesses
{
    public partial class UserAccessService : IUserAccessService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public UserAccessService(
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

        public ValueTask<UserAccess> AddUserAccessAsync(UserAccess userAccess) =>
        TryCatch(async () =>
        {
            UserAccess userAccessWithAddAuditApplied = await ApplyAddAuditAsync(userAccess);
            await ValidateUserAccessOnAddAsync(userAccessWithAddAuditApplied);

            return await this.reIdentificationStorageBroker.InsertUserAccessAsync(userAccessWithAddAuditApplied);
        });

        public ValueTask<IQueryable<UserAccess>> RetrieveAllUserAccessesAsync() =>
        TryCatch(this.reIdentificationStorageBroker.SelectAllUserAccessesAsync);

        public ValueTask<UserAccess> RetrieveUserAccessByIdAsync(Guid userAccessId) =>
        TryCatch(async () =>
        {
            ValidateUserAccessOnRetrieveById(userAccessId);

            var maybeUserAccess = await this.reIdentificationStorageBroker
                .SelectUserAccessByIdAsync(userAccessId);

            ValidateStorageUserAccess(maybeUserAccess, userAccessId);

            return maybeUserAccess;
        });

        public ValueTask<UserAccess> ModifyUserAccessAsync(UserAccess userAccess) =>
        TryCatch(async () =>
        {
            UserAccess userAccessWithModifyAuditApplied = await ApplyModifyAuditAsync(userAccess);
            await ValidateUserAccessOnModifyAsync(userAccessWithModifyAuditApplied);

            var maybeUserAccess = await this.reIdentificationStorageBroker
                .SelectUserAccessByIdAsync(userAccessWithModifyAuditApplied.Id);

            ValidateStorageUserAccess(maybeUserAccess, userAccessWithModifyAuditApplied.Id);
            await ValidateAgainstStorageUserAccessOnModifyAsync(userAccessWithModifyAuditApplied, maybeUserAccess);

            return await this.reIdentificationStorageBroker.UpdateUserAccessAsync(userAccessWithModifyAuditApplied);
        });

        public ValueTask<UserAccess> RemoveUserAccessByIdAsync(Guid userAccessId) =>
        TryCatch(async () =>
        {
            ValidateUserAccessOnRemoveById(userAccessId);

            var maybeUserAccess = await this.reIdentificationStorageBroker
                .SelectUserAccessByIdAsync(userAccessId);

            ValidateStorageUserAccess(maybeUserAccess, userAccessId);
            UserAccess userAccessWithDeleteAuditApplied = await ApplyDeleteAuditAsync(maybeUserAccess);

            var updatedUserAccess = await this.reIdentificationStorageBroker
                .UpdateUserAccessAsync(userAccessWithDeleteAuditApplied);

            await ValidateAgainstStorageUserAccessOnDeleteAsync(updatedUserAccess, userAccessWithDeleteAuditApplied);

            return await this.reIdentificationStorageBroker.DeleteUserAccessAsync(updatedUserAccess);
        });

        public ValueTask<List<string>> RetrieveAllActiveOrganisationsUserHasAccessToAsync(string entraUserId) =>
        TryCatch(async () =>
        {
            ValidateOnRetrieveAllOrganisationUserHasAccessTo(entraUserId);
            List<string> organisations = new List<string>();
            var userAccessQuery = await this.reIdentificationStorageBroker.SelectAllUserAccessesAsync();
            DateTimeOffset currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            List<string> userOrganisations = userAccessQuery
                .Where(userAccess => userAccess.EntraUserId == entraUserId)
                    .Select(userAccess => userAccess.OrgCode).Distinct().ToList();

            foreach (var userOrganisation in userOrganisations)
            {
                IQueryable<OdsData> odsParentRecord =
                    await this.reIdentificationStorageBroker.SelectAllOdsDatasAsync();

                OdsData parentRecord = odsParentRecord
                    .FirstOrDefault(ods => ods.OrganisationCode == userOrganisation);

                if (parentRecord != null)
                {
                    organisations.Add(parentRecord.OrganisationCode);

                    IQueryable<OdsData> odsDataQuery =
                        await this.reIdentificationStorageBroker.SelectAllOdsDatasAsync();

                    odsDataQuery = odsDataQuery
                        .Where(ods => ods.OdsHierarchy.IsDescendantOf(parentRecord.OdsHierarchy)
                            && (ods.RelationshipWithParentStartDate == null
                                || ods.RelationshipWithParentStartDate <= currentDateTime)
                            && (ods.RelationshipWithParentEndDate == null ||
                                ods.RelationshipWithParentEndDate > currentDateTime));

                    List<string> descendants = odsDataQuery.ToList()
                        .Select(odsData => odsData.OrganisationCode).ToList();

                    organisations.AddRange(descendants);
                }
            }

            return organisations.Distinct().ToList();
        });

        virtual internal async ValueTask<UserAccess> ApplyAddAuditAsync(UserAccess userAccess)
        {
            ValidateUserAccessIsNotNull(userAccess);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            userAccess.CreatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            userAccess.CreatedDate = auditDateTimeOffset;
            userAccess.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            userAccess.UpdatedDate = auditDateTimeOffset;

            return userAccess;
        }

        virtual internal async ValueTask<UserAccess> ApplyModifyAuditAsync(UserAccess userAccess)
        {
            ValidateUserAccessIsNotNull(userAccess);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            userAccess.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            userAccess.UpdatedDate = auditDateTimeOffset;

            return userAccess;
        }

        virtual internal async ValueTask<UserAccess> ApplyDeleteAuditAsync(UserAccess userAccess)
        {
            ValidateUserAccessIsNotNull(userAccess);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            userAccess.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            userAccess.UpdatedDate = auditDateTimeOffset;
            return userAccess;
        }
    }
}
