// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.AccessAudits;
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
            OdsData randomOdsData = await PostRandomOdsDataAsync();
            PdsData pdsData = await PostPdsDataAsync(randomOdsData.OrganisationCode, randomOdsData.OrganisationName);
            string securityOid = TestAuthHandler.SecurityOid;

            UserAccess randomUserAccess =
                await PostRandomUserAccessAsync(randomOdsData.OrganisationCode, securityOid);

            AccessRequest randomAccessRequest =
                CreateIdentificationRequestAccessRequestGivenPsuedoId(securityOid, pdsData.PseudoNhsNumber);

            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = inputAccessRequest.DeepClone();
            int expectedHasNoAccessAuditCount = 1;
            int expectedHasAccessAuditCount = 2;
            int expectedHasNoAccessIdentificationItemsCount = 1;
            int expectedHasAccessIdentificationItemsCount = 1;

            // when
            AccessRequest actualAccessRequest =
                await this.apiBroker.PostIdentificationRequestsAsync(inputAccessRequest);

            int actualHasNoAccessIdentificationItemsCount =
                actualAccessRequest.IdentificationRequest.IdentificationItems
                    .Where(item => item.HasAccess == false).Count();

            int actualHasAccessIdentificationItemsCount =
                actualAccessRequest.IdentificationRequest.IdentificationItems
                    .Where(item => item.HasAccess == true).Count();

            List<AccessAudit> accessAudits = await this.apiBroker.GetAllAccessAuditsAsync();

            int actualHasNoAccessAuditsCount = accessAudits.Where(accessAudit => accessAudit.HasAccess == false
                && accessAudit.RequestId == inputAccessRequest.IdentificationRequest.Id)
                    .Count();

            int actualHasAccessAuditsCount = accessAudits.Where(accessAudit => accessAudit.HasAccess == true
                && accessAudit.RequestId == inputAccessRequest.IdentificationRequest.Id)
                    .Count();

            // then
            actualHasNoAccessIdentificationItemsCount.Should().Be(expectedHasNoAccessIdentificationItemsCount);
            actualHasAccessIdentificationItemsCount.Should().Be(expectedHasAccessIdentificationItemsCount);
            actualHasNoAccessAuditsCount.Should().Be(expectedHasNoAccessAuditCount);
            actualHasAccessAuditsCount.Should().Be(expectedHasAccessAuditCount);

            List<AccessAudit> requestRelatedAccesAudits =
                accessAudits.Where(accessAudit => accessAudit.RequestId == randomAccessRequest.IdentificationRequest.Id)
                    .ToList();

            foreach (AccessAudit accessAudit in requestRelatedAccesAudits)
            {
                await this.apiBroker.DeleteAccessAuditByIdAsync(accessAudit.Id);
            }

            await this.apiBroker.DeleteOdsDataByIdAsync(randomOdsData.Id);
            await this.apiBroker.DeletePdsDataByIdAsync(pdsData.Id);
            await this.apiBroker.DeleteUserAccessByIdAsync(randomUserAccess.Id);
        }
    }
}
