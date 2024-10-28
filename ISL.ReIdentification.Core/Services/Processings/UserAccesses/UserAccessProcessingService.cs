// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAccesses
{
    public partial class UserAccessProcessingService : IUserAccessProcessingService
    {
        private readonly IUserAccessService userAccessService;
        private readonly ILoggingBroker loggingBroker;

        public UserAccessProcessingService(IUserAccessService userAccessService, ILoggingBroker loggingBroker)
        {
            this.userAccessService = userAccessService;
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

        public ValueTask<BulkUserAccess> BulkAddRemoveUserAccessAsync(BulkUserAccess bulkUserAccess) =>
            throw new NotImplementedException();
    }
}
