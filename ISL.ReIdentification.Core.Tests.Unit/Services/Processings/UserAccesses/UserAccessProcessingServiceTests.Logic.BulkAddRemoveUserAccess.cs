// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Processings.UserAccesses
{
    public partial class UserAccessProcessingServiceTests
    {
        [Fact]
        public async Task ShouldBulkAddRemoveUserAccessAsync()
        {
            // given
            Guid randomEntraId = Guid.NewGuid();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            List<UserAccess> randomNewUserAccess = CreateRandomUserAccesses(
                randomDateTimeOffset,
                randomEntraId);

            List<UserAccess> randomExistingUserAccess = CreateRandomUserAccesses(
                randomDateTimeOffset,
                randomEntraId);

            List<UserAccess> randomRemovedUserAccess = CreateRandomUserAccesses(
                randomDateTimeOffset,
                randomEntraId);

            List<UserAccess> userAccessesThatShouldExist = new List<UserAccess>();
            userAccessesThatShouldExist.AddRange(randomNewUserAccess);
            userAccessesThatShouldExist.AddRange(randomExistingUserAccess);


            List<string> orgCodesThatShouldExist = new List<string>();
            orgCodesThatShouldExist.AddRange(randomNewUserAccess.Select(userAccess => userAccess.OrgCode).ToList());
            orgCodesThatShouldExist.AddRange(randomExistingUserAccess.Select(userAccess => userAccess.OrgCode).ToList());

            BulkUserAccess bulkUserAccess = new BulkUserAccess
            {
                EntraUserId = randomNewUserAccess.First().EntraUserId,
                GivenName = randomNewUserAccess.First().GivenName,
                Surname = randomNewUserAccess.First().Surname,
                DisplayName = randomNewUserAccess.First().DisplayName,
                JobTitle = randomNewUserAccess.First().JobTitle,
                Email = randomNewUserAccess.First().Email,
                UserPrincipalName = randomNewUserAccess.First().UserPrincipalName,
                OrgCodes = orgCodesThatShouldExist
            };

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            //foreach (UserAccess newUserAccess in randomNewUserAccess)
            //{
            //    this.userAccessServiceMock.Setup(service =>
            //        service.AddUserAccessAsync(newUserAccess))
            //            .ReturnsAsync(newUserAccess);
            //}

            //foreach (UserAccess removedUserAccess in randomRemovedUserAccess)
            //{
            //    this.userAccessServiceMock.Setup(service =>
            //        service.RemoveUserAccessByIdAsync(existingUserAccess.Id))
            //            .ReturnsAsync(existingUserAccess);
            //}


            //this.userAccessServiceMock.Setup(service =>
            //    service.AddUserAccessAsync(inputUserAccess))
            //        .ReturnsAsync(storageUserAccess);

            // when
            UserAccess actualUserAccess = await this.userAccessProcessingService
                .BulkAddRemoveUserAccessAsync(bulkUserAccess);

            //// then
            //actualUserAccess.Should().BeEquivalentTo(expectedUserAccess);

            //this.userAccessServiceMock.Verify(service =>
            //    service.AddUserAccessAsync(inputUserAccess),
            //        Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
