// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.AccessAudits;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.CsvIdentificationRequests;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.OdsDatas;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.PdsDatas;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.UserAccesses;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Apis
{
    public partial class ReIdentificationTests
    {
        [Fact]
        public async Task ShouldPostCsvIdentificationRequestsAsync()
        {
            // given
            string pseudoIdentifier = "0000000001";
            OdsData randomOdsData = await PostRandomOdsDataAsync();
            PdsData pdsData = await PostPdsDataAsync(randomOdsData.OrganisationCode, randomOdsData.OrganisationName);
            Guid securityOid = TestAuthHandler.SecurityOid;

            UserAccess randomUserAccess =
                await PostRandomUserAccessAsync(randomOdsData.OrganisationCode, securityOid);

            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            CsvIdentificationRequest inputCsvIdentificationRequest = randomCsvIdentificationRequest;
            inputCsvIdentificationRequest.RecipientEntraUserId = securityOid;
            inputCsvIdentificationRequest.HasHeaderRecord = false;
            inputCsvIdentificationRequest.IdentifierColumnIndex = 0;
            inputCsvIdentificationRequest.Data = Encoding.UTF8.GetBytes(pseudoIdentifier);
            string randomString = GetRandomStringWithLengthOf(GetRandomNumber());
            string inputReason = randomString;

            CsvIdentificationRequest exisingCsvIdentificationRequest =
                await this.apiBroker.PostCsvIdentificationRequestAsync(inputCsvIdentificationRequest);

            Guid inputCsvIdentificationRequestId = exisingCsvIdentificationRequest.Id;
            int expectedHasAccessAuditCount = 2;

            // when
            byte[] actualFileContentResult =
                await this.apiBroker.PostCsvIdentificationRequestAsync(inputCsvIdentificationRequestId, inputReason);

            List<AccessAudit> accessAudits = await this.apiBroker.GetAllAccessAuditsAsync();

            int actualHasAccessAuditsCount = accessAudits.Where(accessAudit => accessAudit.HasAccess == true)
                .Count();

            // then
            actualHasAccessAuditsCount.Should().Be(expectedHasAccessAuditCount);

            // Uses IdentificationRequestId rather than CsvIdentificationRequestId
            // ALSO Returns NoAccess so need to look at the setup above to ensure the user does have access
            List<AccessAudit> requestRelatedAccesAudits =
                accessAudits.Where(accessAudit => accessAudit.RequestId == exisingCsvIdentificationRequest.Id)
                    .ToList();

            foreach (AccessAudit accessAudit in requestRelatedAccesAudits)
            {
                await this.apiBroker.DeleteAccessAuditByIdAsync(accessAudit.Id);
            }

            await this.apiBroker.DeleteOdsDataByIdAsync(randomOdsData.Id);
            await this.apiBroker.DeletePdsDataByIdAsync(pdsData.Id);
            await this.apiBroker.DeleteUserAccessByIdAsync(randomUserAccess.Id);
            await this.apiBroker.DeleteCsvIdentificationRequestByIdAsync(exisingCsvIdentificationRequest.Id);
        }
    }
}
