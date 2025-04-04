﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.UserAccess;

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string userAccessesRelativeUrl = "api/useraccesses";

        public async ValueTask<UserAccess> PostUserAccessAsync(UserAccess userAccess) =>
            await this.apiFactoryClient.PostContentAsync(userAccessesRelativeUrl, userAccess);

        public async ValueTask PostBulkUserAccessAsync(BulkUserAccess bulkUserAccess) =>
            await this.apiFactoryClient
                .PostContentWithNoResponseAsync($"{userAccessesRelativeUrl}/bulk", bulkUserAccess);

        public async ValueTask<List<UserAccess>> GetAllUserAccessesAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<UserAccess>>($"{userAccessesRelativeUrl}/");

        public async ValueTask<List<UserAccess>> GetSpecificUserAccessByIdAsync(Guid userAccessId) =>
            await this.apiFactoryClient.GetContentAsync<List<UserAccess>>(
                $"{userAccessesRelativeUrl}?$filter=Id eq {userAccessId}");

        public async ValueTask<UserAccess> GetUserAccessByEntraUserIdAndOrgCodeAsync(string entraUserId, string orgCode)
        {
            var results = await this.apiFactoryClient
                .GetContentAsync<List<UserAccess>>($"{userAccessesRelativeUrl}/" +
                    $"?$filter=entraUserId eq '{entraUserId}' and orgCode eq '{orgCode}'");

            return results.FirstOrDefault();
        }

        public async ValueTask<UserAccess> GetUserAccessByIdAsync(Guid userAccessId) =>
            await this.apiFactoryClient.GetContentAsync<UserAccess>($"{userAccessesRelativeUrl}/{userAccessId}");

        public async ValueTask<UserAccess> PutUserAccessAsync(UserAccess userAccess) =>
            await this.apiFactoryClient.PutContentAsync(userAccessesRelativeUrl, userAccess);

        public async ValueTask<UserAccess> DeleteUserAccessByIdAsync(Guid userAccessId) =>
            await this.apiFactoryClient.DeleteContentAsync<UserAccess>($"{userAccessesRelativeUrl}/{userAccessId}");
    }
}
