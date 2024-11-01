// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.UserAccesses;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    public partial class UserAccessesApiTests
    {
        [Fact]
        public async Task ShouldPostUserAccessAsync()
        {
            // given
            UserAccess randomUserAccess = CreateRandomUserAccess();
            UserAccess inputUserAccess = randomUserAccess;
            UserAccess expectedUserAccess = inputUserAccess;

            // when
            await this.apiBroker.PostUserAccessAsync(inputUserAccess);

            UserAccess actualUserAccess =
                await this.apiBroker.GetUserAccessByIdAsync(inputUserAccess.Id);

            // then
            actualUserAccess.Should().BeEquivalentTo(expectedUserAccess);
            await this.apiBroker.DeleteUserAccessByIdAsync(inputUserAccess.Id);
        }

        [Fact]
        public async Task ShouldPostBulkUserAccessAsync()
        {
            // given
            List<string> removeItemsBulkUserAccess = GetRandomStringsWithLengthOf(15, 1);
            List<string> unmodifiedItemsBulkUserAccess = GetRandomStringsWithLengthOf(15, 1);
            List<string> newItemsBulkUserAccess = GetRandomStringsWithLengthOf(15, 1);
            BulkUserAccess setupUserAccesses = CreateRandomBulkUserAccess();
            setupUserAccesses.OrgCodes = new List<string>();
            setupUserAccesses.OrgCodes.AddRange(removeItemsBulkUserAccess);
            setupUserAccesses.OrgCodes.AddRange(unmodifiedItemsBulkUserAccess);
            BulkUserAccess inputUserAccesses = setupUserAccesses.DeepClone();
            inputUserAccesses.OrgCodes = new List<string>();
            inputUserAccesses.OrgCodes.AddRange(newItemsBulkUserAccess);
            inputUserAccesses.OrgCodes.AddRange(unmodifiedItemsBulkUserAccess);
            await SetupBulkUserAccessesAsync(setupUserAccesses);

            // when
            await this.apiBroker.PostBulkUserAccessAsync(inputUserAccesses);

            // then
            foreach (var item in removeItemsBulkUserAccess)
            {
                var userAccess = await this.apiBroker.GetUserAccessByEntraUserIdAndOrgCodeAsync(
                    entraUserId: setupUserAccesses.EntraUserId.ToString(),
                    orgCode: item);

                userAccess.Should().BeNull();
            }

            foreach (var item in inputUserAccesses.OrgCodes)
            {
                var userAccess = await this.apiBroker.GetUserAccessByEntraUserIdAndOrgCodeAsync(
                    entraUserId: setupUserAccesses.EntraUserId.ToString(),
                    orgCode: item);

                userAccess.Should().NotBeNull();
                await this.apiBroker.DeleteUserAccessByIdAsync(userAccess.Id);
            }
        }

        [Fact]
        public async Task ShouldGetAllUserAccessesAsync()
        {
            // given
            List<UserAccess> randomUserAccesses = await PostRandomUserAccesses();
            List<UserAccess> expectedUserAccesses = randomUserAccesses;

            // when
            List<UserAccess> actualUserAccesses = await this.apiBroker.GetAllUserAccessesAsync();

            // then
            foreach (var expectedUserAccess in expectedUserAccesses)
            {
                UserAccess actualUserAccess = actualUserAccesses.Single(
                    actual => actual.Id == expectedUserAccess.Id);

                actualUserAccess.Should().BeEquivalentTo(expectedUserAccess);
                await this.apiBroker.DeleteUserAccessByIdAsync(actualUserAccess.Id);
            }
        }

        [Fact]
        public async Task ShouldGetUserAccessByIdAsync()
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

        [Fact]
        public async Task ShouldPutUserAccessAsync()
        {
            // given
            UserAccess randomUserAccess = await PostRandomUserAccess();
            UserAccess updatedUserAccess = UpdateUserAccess(randomUserAccess);

            // when
            await this.apiBroker.PutUserAccessAsync(updatedUserAccess);
            UserAccess actualUserAccess = await this.apiBroker.GetUserAccessByIdAsync(randomUserAccess.Id);

            // then
            actualUserAccess.Should().BeEquivalentTo(updatedUserAccess);
            await this.apiBroker.DeleteUserAccessByIdAsync(actualUserAccess.Id);
        }

        [Fact]
        public async Task ShouldDeleteUserAccessAsync()
        {
            // given
            UserAccess randomUserAccess = await PostRandomUserAccess();
            UserAccess expectedDeletedUserAccess = randomUserAccess;

            // when
            UserAccess actualUserAccess = await this.apiBroker.DeleteUserAccessByIdAsync(expectedDeletedUserAccess.Id);

            ValueTask<UserAccess> getUserAccessTask =
                this.apiBroker.GetUserAccessByIdAsync(expectedDeletedUserAccess.Id);

            //then
            actualUserAccess.Should().BeEquivalentTo(expectedDeletedUserAccess);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(
                testCode: getUserAccessTask.AsTask);
        }
    }
}
