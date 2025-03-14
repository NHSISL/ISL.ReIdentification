// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.PdsData;

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.Apis.PdsDatas
{
    public partial class PdsDataApiTests
    {
        [Fact]
        public async Task ShouldPutPdsDataAsync()
        {
            // given
            PdsData randomPdsData = await PostRandomPdsDataAsync();
            PdsData modifiedPdsData = UpdatePdsDataWithRandomValues(randomPdsData);

            // when
            await this.apiBroker.PutPdsDataAsync(modifiedPdsData);
            PdsData actualPdsData = await this.apiBroker.GetPdsDataByIdAsync(randomPdsData.Id);

            // then
            actualPdsData.Should().BeEquivalentTo(modifiedPdsData);
            await this.apiBroker.DeletePdsDataByIdAsync(actualPdsData.Id);
        }
    }
}
