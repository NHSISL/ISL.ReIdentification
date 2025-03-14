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
        public async Task ShouldPostUserAccessAsync()
        {
            // given
            UserAccess randomUserAccess = CreateRandomUserAccess();
            UserAccess expectedUserAccess = randomUserAccess;

            // when 
            await this.apiBroker.PostUserAccessAsync(randomUserAccess);

            UserAccess actualUserAccess =
                await this.apiBroker.GetUserAccessByIdAsync(randomUserAccess.Id);

            // then
            actualUserAccess.Should().BeEquivalentTo(
                expectedUserAccess,
                options => options
                    .Excluding(userAccess => userAccess.CreatedBy)
                    .Excluding(userAccess => userAccess.CreatedDate)
                    .Excluding(userAccess => userAccess.UpdatedBy)
                    .Excluding(userAccess => userAccess.UpdatedDate));

            await this.apiBroker.DeleteUserAccessByIdAsync(actualUserAccess.Id);
        }
    }
}
