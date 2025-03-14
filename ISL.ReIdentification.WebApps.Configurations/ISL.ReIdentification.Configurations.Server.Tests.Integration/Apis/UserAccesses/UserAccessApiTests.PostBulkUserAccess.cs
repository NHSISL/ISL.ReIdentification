// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.UserAccess;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis.UserAccesses
{
    public partial class UserAccessApiTests
    {
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
    }
}