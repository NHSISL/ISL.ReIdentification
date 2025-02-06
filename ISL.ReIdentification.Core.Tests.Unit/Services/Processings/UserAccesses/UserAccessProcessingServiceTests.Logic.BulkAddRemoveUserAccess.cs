// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Processings.UserAccesses
{
    public partial class UserAccessProcessingServiceTests
    {
        [Fact]
        public async Task ShouldBulkAddRemoveUserAccessAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid randomEntraId = Guid.NewGuid();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            EntraUser currentUser = new EntraUser(
                entraUserId: randomEntraId,
                givenName: GetRandomString(),
                surname: GetRandomString(),
                displayName: GetRandomString(),
                email: GetRandomString(),
                jobTitle: GetRandomString(),
                roles: new List<string>(),
                claims: new List<Claim>());

            List<UserAccess> randomNewUserAccess = CreateRandomUserAccesses(
                dateTimeOffset: randomDateTimeOffset,
                entraUserId: randomEntraId,
                count: GetRandomNumber());

            List<UserAccess> randomExistingUserAccesses = CreateRandomUserAccesses(
                dateTimeOffset: randomDateTimeOffset,
                entraUserId: randomEntraId,
                count: GetRandomNumber());

            List<UserAccess> randomRemovedUserAccesses = CreateRandomUserAccesses(
                dateTimeOffset: randomDateTimeOffset,
                entraUserId: randomEntraId,
                count: GetRandomNumber());

            randomNewUserAccess.ForEach(userAccess =>
            {
                userAccess.Id = randomId;
                userAccess.EntraUserId = randomNewUserAccess.First().EntraUserId;
                userAccess.GivenName = randomNewUserAccess.First().GivenName;
                userAccess.Surname = randomNewUserAccess.First().Surname;
                userAccess.DisplayName = randomNewUserAccess.First().DisplayName;
                userAccess.JobTitle = randomNewUserAccess.First().JobTitle;
                userAccess.Email = randomNewUserAccess.First().Email;
                userAccess.UserPrincipalName = randomNewUserAccess.First().UserPrincipalName;
                userAccess.CreatedBy = currentUser.EntraUserId.ToString();
                userAccess.CreatedDate = randomDateTimeOffset;
                userAccess.UpdatedBy = currentUser.EntraUserId.ToString();
                userAccess.UpdatedDate = randomDateTimeOffset;
            });

            List<UserAccess> newUsers = randomNewUserAccess.DeepClone();

            List<UserAccess> storageUserAccess = new List<UserAccess>();
            storageUserAccess.AddRange(randomExistingUserAccesses);
            storageUserAccess.AddRange(randomRemovedUserAccesses);

            List<string> validOrgCodes = new List<string>();
            validOrgCodes.AddRange(randomNewUserAccess.Select(userAccess => userAccess.OrgCode).ToList());
            validOrgCodes.AddRange(randomExistingUserAccesses.Select(userAccess => userAccess.OrgCode).ToList());

            BulkUserAccess inputBulkUserAccess = new BulkUserAccess
            {
                EntraUserId = randomNewUserAccess.First().EntraUserId,
                GivenName = randomNewUserAccess.First().GivenName,
                Surname = randomNewUserAccess.First().Surname,
                DisplayName = randomNewUserAccess.First().DisplayName,
                JobTitle = randomNewUserAccess.First().JobTitle,
                Email = randomNewUserAccess.First().Email,
                UserPrincipalName = randomNewUserAccess.First().UserPrincipalName,
                OrgCodes = validOrgCodes
            };

            this.userAccessServiceMock.Setup(service =>
                service.RetrieveAllUserAccessesAsync())
                    .ReturnsAsync(storageUserAccess.AsQueryable());

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                        .ReturnsAsync(randomId);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(currentUser);

            // when
            await this.userAccessProcessingService.BulkAddRemoveUserAccessAsync(inputBulkUserAccess);

            // then
            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllUserAccessesAsync(),
                    Times.Once);

            foreach (UserAccess userAccess in randomRemovedUserAccesses)
            {
                this.userAccessServiceMock.Verify(service =>
                    service.RemoveUserAccessByIdAsync(userAccess.Id),
                        Times.Once);
            }

            foreach (UserAccess newUser in newUsers)
            {
                this.userAccessServiceMock.Verify(service =>
                    service.AddUserAccessAsync(It.Is(SameUserAccessAs(newUser))),
                        Times.Once);
            }

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(randomNewUserAccess.Count));

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Exactly(randomNewUserAccess.Count));

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
