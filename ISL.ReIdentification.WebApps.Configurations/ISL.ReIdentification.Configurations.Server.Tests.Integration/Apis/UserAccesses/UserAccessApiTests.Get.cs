// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.UserAccess;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis.UserAccesses
{
    public partial class UserAccessApiTests
    {
        [Fact]
        public async Task ShouldGetAllUserAccessAsync()
        {
            // given
            List<UserAccess> randomUserAccess = await PostRandomUserAccesses();
            List<UserAccess> expectedUserAccesses = randomUserAccess;

            // when
            List<UserAccess> actualUserAccesses = await this.apiBroker.GetAllUserAccessesAsync();

            // then
            actualUserAccesses.Should().NotBeNull();

            foreach (UserAccess expectedUserAccess in expectedUserAccesses)
            {
                UserAccess actualUserAccess = actualUserAccesses
                    .Single(lookup => lookup.Id == expectedUserAccess.Id);

                actualUserAccess.Should().BeEquivalentTo(
                    expectedUserAccess,
                    options => options
                        .Excluding(property => property.CreatedBy)
                        .Excluding(property => property.CreatedDate)
                        .Excluding(property => property.UpdatedBy)
                        .Excluding(property => property.UpdatedDate));
            }

            // cleanup
            foreach (UserAccess createdUserAccess in expectedUserAccesses)
            {
                await this.apiBroker.DeleteUserAccessByIdAsync(createdUserAccess.Id);
            }
        }
    }
}
