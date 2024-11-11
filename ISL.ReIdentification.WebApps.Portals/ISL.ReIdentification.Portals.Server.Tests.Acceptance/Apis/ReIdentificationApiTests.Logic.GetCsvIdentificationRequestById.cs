// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Text;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.CsvIdentificationRequests;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.OdsDatas;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.PdsDatas;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.UserAccesses;
using Microsoft.AspNetCore.Mvc;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    public partial class ReIdentificationApiTests
    {
        [Fact]
        public async Task ShouldGetCsvIdentificationRequestByIdAsync()
        {
            // given
            string pseudoIdentifier = "0000000001";
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            CsvIdentificationRequest inputCsvIdentificationRequest = randomCsvIdentificationRequest;
            inputCsvIdentificationRequest.HasHeaderRecord = false;
            inputCsvIdentificationRequest.IdentifierColumnIndex = 0;

            /// Add equivalent byte array as single line csv with pseudo identifier
            inputCsvIdentificationRequest.Data = Encoding.UTF8.GetBytes(pseudoIdentifier);

            string randomString = GetRandomStringWithLengthOf(GetRandomNumber());
            string inputReason = randomString;

            CsvIdentificationRequest exisingCsvIdentificationRequest =
                await this.apiBroker.PostCsvIdentificationRequestAsync(inputCsvIdentificationRequest);

            /// Same Guid as the Oid set in TestAuthHandler
            string testOidGuid = "efc48de6-420f-44a8-8e41-bf1e1793da8d";
            Guid entraUserId = Guid.Parse(testOidGuid);

            /// Create a  User Access with same EntraUserId with some organisation OrgCode. 
            UserAccess createdUserAccess = await PostRandomUserAccess(entraUserId);

            ///Create odsData with the OrganisationCode
            OdsData createdOdsData = await PostRandomOdsData(createdUserAccess.OrgCode);

            /// pds contains orgCode and pseudoIdentifier
            PdsData createdPdsData = await PostRandomPdsDataAsync(pseudoIdentifier, createdUserAccess.OrgCode);

            // when
            ActionResult actualResult =
                await this.apiBroker.GetCsvIdentificationRequestByIdAsync(exisingCsvIdentificationRequest.Id, inputReason);

            // then
            /// Add some assertion e.g. the resulting identified NHS number = "1111111111"
            await this.apiBroker.DeleteCsvIdentificationRequestByIdAsync(exisingCsvIdentificationRequest.Id);
            await this.apiBroker.DeleteUserAccessByIdAsync(createdUserAccess.Id);
            await this.apiBroker.DeleteOdsDataByIdAsync(createdOdsData.Id);
            await this.apiBroker.DeletePdsDataByIdAsync(createdPdsData.Id);
        }
    }
}
