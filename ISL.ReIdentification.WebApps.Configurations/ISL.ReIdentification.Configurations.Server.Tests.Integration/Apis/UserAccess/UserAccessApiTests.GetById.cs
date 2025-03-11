// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.UserAccess;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis
{
    public partial class UserAccessApiTests
    {
        [Fact]
        public async Task ShouldGetUserAccessAsync()
        {
            // given
            UserAccess randomUserAccess = await PostRandomUserAccess();
            UserAccess expectedUserAccess = randomUserAccess;

            // when
            UserAccess actualUserAccess = await this.apiBroker.GetUserAccessByIdAsync(randomUserAccess.Id);

            // then
            actualUserAccess.Should().BeEquivalentTo(expectedUserAccess);
            await this.apiBroker.DeleteUserAccessByIdAsync(actualUserAccess.Id);
        }
    }
}
