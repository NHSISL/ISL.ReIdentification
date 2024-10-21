// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.Lookups;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string userAccessesRelativeUrl = "api/useraccesses";

        public async ValueTask<UserAccess> PostUserAccessAsync(UserAccess userAccess) =>
            await this.apiFactoryClient.PostContentAsync(userAccessesRelativeUrl, userAccess);

        public async ValueTask<List<UserAccess>> GetAllUserAccessesAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<UserAccess>>($"{userAccessesRelativeUrl}/");

        public async ValueTask<UserAccess> GetUserAccessByIdAsync(Guid userAccessId) =>
            await this.apiFactoryClient.GetContentAsync<UserAccess>($"{userAccessesRelativeUrl}/{userAccessId}");

        public async ValueTask<UserAccess> PutUserAccessAsync(UserAccess userAccess) =>
            await this.apiFactoryClient.PutContentAsync(userAccessesRelativeUrl, userAccess);

        public async ValueTask<UserAccess> DeleteUserAccessByIdAsync(Guid userAccessId) =>
            await this.apiFactoryClient.DeleteContentAsync<UserAccess>($"{userAccessesRelativeUrl}/{userAccessId}");
    }
}
