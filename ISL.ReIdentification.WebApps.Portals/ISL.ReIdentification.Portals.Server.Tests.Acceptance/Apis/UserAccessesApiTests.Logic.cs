// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.UserAccesses;
using RESTFulSense.Exceptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
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
            BulkUserAccess randomBulkUserAccess = CreateRandomBulkUserAccess();
            BulkUserAccess inputBulkUserAccess = randomBulkUserAccess;

            // when
            await this.apiBroker.PostBulkUserAccessAsync(inputBulkUserAccess);
            List<UserAccess> actualUserAccesses = await this.apiBroker.GetAllUserAccessesAsync();

            // then
            actualUserAccesses.Should().Contain(ua => ua.EntraUserId == inputBulkUserAccess.EntraUserId);

            foreach (UserAccess actualUserAccess in actualUserAccesses)
            {
                if (actualUserAccess.EntraUserId == inputBulkUserAccess.EntraUserId)
                {
                    await this.apiBroker.DeleteUserAccessByIdAsync(actualUserAccess.Id);
                }
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
