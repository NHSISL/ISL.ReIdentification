// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAccesses
{
    public partial class UserAccessesTests
    {
        [Fact]
        public async Task ShouldRetrieveAllActiveOrganisationsUserHasAccessToAsync()
        {
            // given
            Guid randomEntraUserId = Guid.NewGuid();
            Guid inputEntraUserId = randomEntraUserId;
            List<UserAccess> validUserAccesses = CreateValidUserAccesses(inputEntraUserId);
            List<UserAccess> invalidUserAccesses = CreateInvalidUserAccesses(inputEntraUserId);
            List<UserAccess> storageUserAccess = new List<UserAccess>();
            storageUserAccess.AddRange(validUserAccesses);
            storageUserAccess.AddRange(invalidUserAccesses);
            List<OdsData> userOdsDatas = new List<OdsData>();

            foreach (var userAccess in storageUserAccess)
            {
                List<OdsData> odsDatas = CreateRandomOdsDatasByOrgCode(userAccess.OrgCode);
                userOdsDatas.AddRange(odsDatas);
            }

            for (var index = 0; index < userOdsDatas.Count; index++)
            {
                userOdsDatas[index].OdsHierarchy = HierarchyId.Parse($"/{index}/");
            }

            List<string> expectedOrganisations = validUserAccesses.Select(userAccess => userAccess.OrgCode).ToList();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllUserAccessesAsync())
                    .ReturnsAsync(storageUserAccess.AsQueryable());

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.UtcNow);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllOdsDatasAsync())
                    .ReturnsAsync(userOdsDatas.AsQueryable());

            // when
            List<string> actualOrganisations = await this.userAccessService
                .RetrieveAllActiveOrganisationsUserHasAccessToAsync(inputEntraUserId);

            // then
            actualOrganisations.Should().BeEquivalentTo(expectedOrganisations);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllUserAccessesAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllOdsDatasAsync(),
                    Times.Exactly(validUserAccesses.Count() * 2));

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
