// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.PdsData;

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.Apis.PdsDatas
{
    public partial class PdsDataApiTests
    {
        [Fact]
        public async Task ShouldGetAllPdsDataAsync()
        {
            // given
            List<PdsData> postedPdsData = await PostRandomPdsDatasAsync();

            // when
            List<PdsData> retrievedPdsData = await this.apiBroker.GetAllPdsDataAsync();

            // then
            foreach (var pdsData in postedPdsData)
            {
                retrievedPdsData.Should().ContainEquivalentOf(pdsData);
                await this.apiBroker.DeletePdsDataByIdAsync(pdsData.Id);
            }
        }
    }
}
