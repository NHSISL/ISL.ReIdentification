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
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAccesses
{
    public partial class UserAccessesTests
    {
        [Theory]
        [MemberData(nameof(OrganisationListsAndExpectedOutputs))]
        public async Task ShouldRetrieveAllActiveOrganisationsUserHasAccessToAsync(
            List<UserAccess> userAccesses,
            List<OdsData> odsDataItems,
            List<string> expectedOrganisations)
        {
            // given
            Guid randomEntraUserId = Guid.NewGuid();
            Guid inputEntraUserId = randomEntraUserId;
            userAccesses.ForEach(userAccess => userAccess.EntraUserId = inputEntraUserId);
            List<UserAccess> storageUserAccess = userAccesses;

            List<UserAccess> activeUserAccesses = storageUserAccess.Where(userAccess =>
                userAccess.ActiveFrom <= DateTimeOffset.UtcNow
                    && (userAccess.ActiveTo == null || userAccess.ActiveTo > DateTimeOffset.UtcNow)).ToList();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllUserAccessesAsync())
                    .ReturnsAsync(storageUserAccess.AsQueryable());

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.UtcNow);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllOdsDatasAsync())
                    .ReturnsAsync(odsDataItems.AsQueryable());

            // when
            List<string> actualOrganisations = await this.userAccessService
                .RetrieveAllActiveOrganisationsUserHasAccessTo(inputEntraUserId);

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
                    Times.Exactly(activeUserAccesses.Count() * 2));

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
