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
        public async Task ShouldGetPdsDataByIdAsync()
        {
            // given
            PdsData randomPdsData = await PostRandomPdsDataAsync();

            // when
            PdsData retrievedPdsData = await this.apiBroker.GetPdsDataByIdAsync(randomPdsData.Id);

            // then
            retrievedPdsData.Should().BeEquivalentTo(randomPdsData);
            await this.apiBroker.DeletePdsDataByIdAsync(randomPdsData.Id);
        }
    }
}