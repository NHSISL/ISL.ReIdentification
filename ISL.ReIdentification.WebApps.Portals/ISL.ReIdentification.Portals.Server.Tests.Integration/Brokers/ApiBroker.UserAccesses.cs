// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------



using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.UserAccesses;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string userAccessesRelativeUrl = "api/useraccesses";

        public async ValueTask<UserAccess> PostUserAccessAsync(UserAccess userAccess) =>
            await this.apiFactoryClient.PostContentAsync(userAccessesRelativeUrl, userAccess);

        public async ValueTask<UserAccess> DeleteUserAccessByIdAsync(Guid userAccessId) =>
            await this.apiFactoryClient.DeleteContentAsync<UserAccess>($"{userAccessesRelativeUrl}/{userAccessId}");
    }
}
