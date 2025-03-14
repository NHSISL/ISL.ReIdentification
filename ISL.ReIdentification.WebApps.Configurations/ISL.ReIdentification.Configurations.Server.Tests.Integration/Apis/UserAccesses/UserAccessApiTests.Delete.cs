// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.UserAccess;
using RESTFulSense.Exceptions;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis.UserAccesses
{
    public partial class UserAccessApiTests
    {
        [Fact]
        public async Task ShouldDeleteUserAccessAsync()
        {
            // given
            UserAccess randomUserAccess = await PostRandomUserAccess();

            // when
            await this.apiBroker.DeleteUserAccessByIdAsync(randomUserAccess.Id);

            // then
            ValueTask<UserAccess> getUserAccessByIdTask = this.apiBroker.GetUserAccessByIdAsync(randomUserAccess.Id);
            await Assert.ThrowsAsync<HttpResponseNotFoundException>(getUserAccessByIdTask.AsTask);
        }
    }
}
