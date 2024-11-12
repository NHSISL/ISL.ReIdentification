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
        [Theory]
        //[InlineData(false)]
        [InlineData(true)]
        public async Task ShouldGetCsvIdentificationRequestByIdAsync(bool hasHeader)
        {
            // given
            string pseudoIdentifier = "0000000001";
            string nhsNumber = "1111111111";
            string randomHeaderValue = GetRandomStringWithLengthOf(10);
            string randomValue = GetRandomStringWithLengthOf(10);
            StringBuilder csvInputData = new StringBuilder();
            StringBuilder csvExpectedData = new StringBuilder();

            if (hasHeader)
            {
                csvInputData.AppendLine(
                    $"{randomHeaderValue}0,{randomHeaderValue}1,{randomHeaderValue}2");

                csvExpectedData.AppendLine(
                    $"{randomHeaderValue}0,{randomHeaderValue}1,{randomHeaderValue}2");
            }

            csvInputData.AppendLine($"{randomValue},{randomValue},{pseudoIdentifier}");
            csvExpectedData.AppendLine($"{randomValue},{randomValue},{nhsNumber}");
            int identifierIndexPosition = 2;
            Guid securityOid = TestAuthHandler.SecurityOid;
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            CsvIdentificationRequest inputCsvIdentificationRequest = randomCsvIdentificationRequest;
            inputCsvIdentificationRequest.HasHeaderRecord = hasHeader;
            inputCsvIdentificationRequest.IdentifierColumnIndex = identifierIndexPosition;
            inputCsvIdentificationRequest.Data = Encoding.UTF8.GetBytes(csvInputData.ToString());
            string expectedResult = csvExpectedData.ToString();
            string randomString = GetRandomStringWithLengthOf(GetRandomNumber());
            string inputReason = randomString;

            CsvIdentificationRequest exisingCsvIdentificationRequest =
                await this.apiBroker.PostCsvIdentificationRequestAsync(inputCsvIdentificationRequest);

            UserAccess createdUserAccess = await PostRandomUserAccess(securityOid);
            OdsData createdOdsData = await PostRandomOdsData(createdUserAccess.OrgCode);

            PdsData createdPdsData =
                await PostRandomPdsDataGivenPseudoAsync(pseudoIdentifier, createdUserAccess.OrgCode);

            // when
            byte[] actualData =
                await this.apiBroker.GetCsvIdentificationRequestByIdAsync(
                    exisingCsvIdentificationRequest.Id, inputReason);

            // then
            string actualResult = Encoding.UTF8.GetString(actualData);
            actualResult.Should().BeEquivalentTo(expectedResult);
            await this.apiBroker.DeleteCsvIdentificationRequestByIdAsync(exisingCsvIdentificationRequest.Id);
            await this.apiBroker.DeleteUserAccessByIdAsync(createdUserAccess.Id);
            await this.apiBroker.DeleteOdsDataByIdAsync(createdOdsData.Id);
            await this.apiBroker.DeletePdsDataByIdAsync(createdPdsData.Id);
        }
    }
}
