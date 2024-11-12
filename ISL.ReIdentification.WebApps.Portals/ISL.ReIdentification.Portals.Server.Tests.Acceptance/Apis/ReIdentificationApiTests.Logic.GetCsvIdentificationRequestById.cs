// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.CsvIdentificationRequests;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.OdsDatas;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.PdsDatas;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.UserAccesses;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    public partial class ReIdentificationApiTests
    {
        [Fact]
        public async Task ShouldGetCsvIdentificationRequestByIdAsync()
        {
            // given
            string pseudoIdentifier = "0000000001";
            string expectedNhsNumber = "1111111111";
            Guid securityOid = TestAuthHandler.SecurityOid;
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            CsvIdentificationRequest inputCsvIdentificationRequest = randomCsvIdentificationRequest;
            inputCsvIdentificationRequest.HasHeaderRecord = false;
            inputCsvIdentificationRequest.IdentifierColumnIndex = 0;
            inputCsvIdentificationRequest.Data = Encoding.UTF8.GetBytes(pseudoIdentifier);
            string randomString = GetRandomStringWithLengthOf(GetRandomNumber());
            string inputReason = randomString;

            CsvIdentificationRequest exisingCsvIdentificationRequest =
                await this.apiBroker.PostCsvIdentificationRequestAsync(inputCsvIdentificationRequest);

            UserAccess createdUserAccess = await PostRandomUserAccess(securityOid);
            OdsData createdOdsData = await PostRandomOdsData(createdUserAccess.OrgCode);

            PdsData createdPdsData =
                await PostRandomPdsDataGivenPseudoAsync(pseudoIdentifier, createdUserAccess.OrgCode);

            // when
            byte[] fileContent =
                await this.apiBroker.GetCsvIdentificationRequestByIdAsync(
                    exisingCsvIdentificationRequest.Id, inputReason);

            string actualNhsNumber = Encoding.UTF8.GetString(fileContent);

            // then
            actualNhsNumber.Should().Be(expectedNhsNumber);
            await this.apiBroker.DeleteCsvIdentificationRequestByIdAsync(exisingCsvIdentificationRequest.Id);
            await this.apiBroker.DeleteUserAccessByIdAsync(createdUserAccess.Id);
            await this.apiBroker.DeleteOdsDataByIdAsync(createdOdsData.Id);
            await this.apiBroker.DeletePdsDataByIdAsync(createdPdsData.Id);
        }
    }
}
