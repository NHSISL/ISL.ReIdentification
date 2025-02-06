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
        [Fact]
        public async Task ShouldRetrieveAllActiveOrganisationsUserHasAccessToAsync()
        {
            // given
            Guid randomEntraUserId = Guid.NewGuid();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Guid inputEntraUserId = randomEntraUserId;
            List<UserAccess> validUserAccesses = CreateUserAccesses(count: GetRandomNumber());
            validUserAccesses.ForEach(userAccess => userAccess.EntraUserId = inputEntraUserId);
            List<UserAccess> invalidUserAccesses = CreateUserAccesses(count: GetRandomNumber());
            List<UserAccess> storageUserAccess = new List<UserAccess>();
            storageUserAccess.AddRange(validUserAccesses);
            storageUserAccess.AddRange(invalidUserAccesses);
            List<OdsData> userOdsDatas = new List<OdsData>();

            foreach (var userAccess in storageUserAccess)
            {
                List<OdsData> odsDatas = CreateRandomOdsDatasByOrgCode(
                    orgCode: userAccess.OrgCode,
                    dateTimeOffset: randomDateTimeOffset,
                    childrenCount: GetRandomNumber());

                userOdsDatas.AddRange(odsDatas);
            }

            List<string> validOrgCodes = validUserAccesses
                .Select(userAccess => userAccess.OrgCode).ToList();

            OdsData validOdsDataItem = userOdsDatas
                .Where(odsData => validOrgCodes.Contains(odsData.OrganisationCode)).FirstOrDefault();

            List<OdsData> expectedOrganisations = userOdsDatas
                .Where(odsData =>
                    (odsData.OrganisationCode == validOdsDataItem.OrganisationCode
                        || odsData.OdsHierarchy.IsDescendantOf(validOdsDataItem.OdsHierarchy))
                    && (odsData.RelationshipWithParentStartDate == null
                        || odsData.RelationshipWithParentStartDate <= randomDateTimeOffset)
                    && (odsData.RelationshipWithParentEndDate == null ||
                        odsData.RelationshipWithParentEndDate > randomDateTimeOffset)).ToList();

            List<string> expectedOrganisationCodes = expectedOrganisations
                .Select(odsData => odsData.OrganisationCode).Distinct().ToList();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllUserAccessesAsync())
                    .ReturnsAsync(storageUserAccess.AsQueryable());

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllOdsDatasAsync())
                    .ReturnsAsync(userOdsDatas.AsQueryable());

            // when
            List<string> actualOrganisationCodes = await this.userAccessService
                .RetrieveAllActiveOrganisationsUserHasAccessToAsync(inputEntraUserId);

            // then
            actualOrganisationCodes.Should().BeEquivalentTo(expectedOrganisationCodes);

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
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
