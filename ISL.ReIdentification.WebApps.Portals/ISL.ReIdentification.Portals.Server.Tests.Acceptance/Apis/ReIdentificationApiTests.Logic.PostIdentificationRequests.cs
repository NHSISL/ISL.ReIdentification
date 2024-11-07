// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.Accesses;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.OdsDatas;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.PdsDatas;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.UserAccesses;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    public partial class ReIdentificationApiTests
    {
        [Fact]
        public async Task ShouldPostIdentificationRequestsAsync()
        {
            // given
            List<OdsData> randomOdsDatas = await PostRandomOdsDatasAsync();
            List<PdsData> pdsDatas = new List<PdsData>();

            foreach (OdsData odsData in randomOdsDatas)
            {
                PdsData pdsData = await PostPdsDataAsync(odsData.OrganisationCode, odsData.OrganisationName);
                pdsDatas.Add(pdsData);
            }

            UserAccess randomUserAccess = await PostRandomUserAccessAsync(randomOdsDatas[0].OrganisationCode);
            Guid entraUserId = randomUserAccess.EntraUserId;
            AccessRequest randomAccessRequest = CreateIdentificationRequestAccessRequestGivenPds(entraUserId, pdsDatas);
            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = inputAccessRequest.DeepClone();

            // when
            AccessRequest actualAccessRequest =
                await this.apiBroker.PostIdentificationRequestsAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            foreach (OdsData odsData in randomOdsDatas)
            {
                await this.apiBroker.DeleteOdsDataByIdAsync(odsData.Id);

            }

            foreach (PdsData pdsData in pdsDatas)
            {
                await this.apiBroker.DeletePdsDataByIdAsync(pdsData.Id);

            }

            await this.apiBroker.DeleteUserAccessByIdAsync(randomUserAccess.Id);
        }
    }
}
