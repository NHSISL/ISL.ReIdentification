// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAccesses
{
    public partial class UserAccessesTests
    {
        [Fact]
        public async Task ShouldRetrieveAllOrganisationsUserHasAccessToAsync()
        {
            // given
            Guid randomEntraUserId = Guid.NewGuid();
            Guid inputEntraUserId = randomEntraUserId;
            List<UserAccess> randomUserAccess = CreateRandomUserAccesses();
            randomUserAccess.ForEach(userAccess => userAccess.EntraUserId = inputEntraUserId);
            List<UserAccess> storageUserAccess = randomUserAccess;

            List<string> randomUserOrganisations = randomUserAccess
                .Select(randomUserAccess => randomUserAccess.OrgCode).ToList();

            List<OdsData> randomOdsDataItems = CreateRandomOdsDatas(randomUserOrganisations);
            List<OdsData> storageOdsDataItems = randomOdsDataItems.DeepClone();
            List<string> allUserOrganisation = randomUserOrganisations;

            foreach (OdsData odsData in randomOdsDataItems)
            {
                List<OdsData> randomOdsDatas = CreateRandomOdsDataChildren(odsData.OdsHierarchy);
                allUserOrganisation.AddRange(randomOdsDatas.Select(odsData => odsData.OrganisationCode));
                storageOdsDataItems.AddRange(randomOdsDatas);
            }

            List<string> expectedOrganisations = allUserOrganisation.Distinct().ToList();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllUserAccessesAsync())
                    .ReturnsAsync(storageUserAccess.AsQueryable());

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllOdsDatasAsync())
                    .ReturnsAsync(storageOdsDataItems.AsQueryable());

            // when
            List<string> actualOrganisations = await this.userAccessService
                .RetrieveAllActiveOrganisationsUserHasAccessToAsync(inputEntraUserId);

            // then
            actualOrganisations.Should().BeEquivalentTo(expectedOrganisations);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllUserAccessesAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllOdsDatasAsync(),
                    Times.Exactly(storageUserAccess.Count() * 2));

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
