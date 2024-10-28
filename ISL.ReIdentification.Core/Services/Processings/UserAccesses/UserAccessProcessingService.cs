// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Identifiers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Securities;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAccesses
{
    public partial class UserAccessProcessingService : IUserAccessProcessingService
    {
        private readonly IUserAccessService userAccessService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly IIdentifierBroker identifierBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public UserAccessProcessingService(
            IUserAccessService userAccessService,
            IDateTimeBroker dateTimeBroker,
            IIdentifierBroker identifierBroker,
            ISecurityBroker securityBroker,
            ILoggingBroker loggingBroker)
        {
            this.userAccessService = userAccessService;
            this.dateTimeBroker = dateTimeBroker;
            this.identifierBroker = identifierBroker;
            this.securityBroker = securityBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<UserAccess> AddUserAccessAsync(UserAccess userAccess) =>
        TryCatch(async () =>
        {
            ValidateOnAddUserAccess(userAccess);

            return await this.userAccessService.AddUserAccessAsync(userAccess);
        });

        public ValueTask<IQueryable<UserAccess>> RetrieveAllUserAccessesAsync() =>
        TryCatch(async () =>
        {
            return await this.userAccessService.RetrieveAllUserAccessesAsync();
        });

        public ValueTask<UserAccess> RetrieveUserAccessByIdAsync(Guid userAccessId) =>
        TryCatch(async () =>
        {
            ValidateOnRetrieveUserAccessById(userAccessId);

            return await this.userAccessService.RetrieveUserAccessByIdAsync(userAccessId);
        });

        public ValueTask<UserAccess> ModifyUserAccessAsync(UserAccess userAccess) =>
        TryCatch(async () =>
        {
            ValidateOnModifyUserAccess(userAccess);

            return await this.userAccessService.ModifyUserAccessAsync(userAccess);
        });

        public ValueTask<UserAccess> RemoveUserAccessByIdAsync(Guid userAccessId) =>
        TryCatch(async () =>
        {
            ValidateOnRemoveUserAccessById(userAccessId);

            return await this.userAccessService.RemoveUserAccessByIdAsync(userAccessId);
        });

        public ValueTask<List<string>> RetrieveAllActiveOrganisationsUserHasAccessToAsync(Guid entraUserId) =>
        TryCatch(async () =>
        {
            ValidateOnRetrieveAllActiveOrganisationsUserHasAccessTo(entraUserId);

            return await this.userAccessService.RetrieveAllActiveOrganisationsUserHasAccessToAsync(entraUserId);
        });

        public async ValueTask BulkAddRemoveUserAccessAsync(BulkUserAccess bulkUserAccess)
        {
            IQueryable<UserAccess> existingUserAccessQuery =
                await this.userAccessService.RetrieveAllUserAccessesAsync();

            List<UserAccess> existingUserAccess = existingUserAccessQuery
                .Where(userAccess => userAccess.EntraUserId == bulkUserAccess.EntraUserId).ToList();


            foreach (UserAccess userAccess in existingUserAccess)
            {
                if (!bulkUserAccess.OrgCodes.Contains(userAccess.OrgCode))
                {
                    await this.userAccessService.RemoveUserAccessByIdAsync(userAccess.Id);
                }
            }

            List<UserAccess> newUserAccess = new List<UserAccess>();
            EntraUser user = await this.securityBroker.GetCurrentUser();
            var existingOrgCodes = new HashSet<string>(existingUserAccess.Select(ua => ua.OrgCode));

            foreach (string orgCode in bulkUserAccess.OrgCodes)
            {
                if (!existingOrgCodes.Contains(orgCode))
                {
                    Guid id = await this.identifierBroker.GetIdentifierAsync();
                    DateTimeOffset now = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

                    newUserAccess.Add(new UserAccess
                    {
                        Id = id,
                        EntraUserId = bulkUserAccess.EntraUserId,
                        GivenName = bulkUserAccess.GivenName,
                        Surname = bulkUserAccess.Surname,
                        DisplayName = bulkUserAccess.DisplayName,
                        JobTitle = bulkUserAccess.JobTitle,
                        Email = bulkUserAccess.Email,
                        UserPrincipalName = bulkUserAccess.UserPrincipalName,
                        OrgCode = orgCode,
                        CreatedBy = user.EntraUserId.ToString(),
                        CreatedDate = now,
                        UpdatedBy = user.EntraUserId.ToString(),
                        UpdatedDate = now
                    });
                }
            }

            foreach (UserAccess userAccess in newUserAccess)
            {
                await this.userAccessService.AddUserAccessAsync(userAccess);
            }
        }
    }
}
