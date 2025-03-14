// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.UserAccess;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis.UserAccesses
{
    public partial class UserAccessApiTests
    {
        [Fact]
        public async Task ShouldPutUserAccessAsync()
        {
            // given
            UserAccess randomUserAccess = await PostRandomUserAccess();
            UserAccess modifiedUserAccess = UpdateUserAccess(randomUserAccess);

            // when
            await this.apiBroker.PutUserAccessAsync(modifiedUserAccess);
            UserAccess actualUserAccess = await this.apiBroker.PutUserAccessAsync(modifiedUserAccess);

            // then
            actualUserAccess.Should().BeEquivalentTo(
                modifiedUserAccess,
                options => options
                    .Excluding(userAccess => userAccess.CreatedBy)
                    .Excluding(userAccess => userAccess.CreatedDate)
                    .Excluding(userAccess => userAccess.UpdatedBy)
                    .Excluding(userAccess => userAccess.UpdatedDate));

            await this.apiBroker.DeleteUserAccessByIdAsync(actualUserAccess.Id);
        }
    }
}
