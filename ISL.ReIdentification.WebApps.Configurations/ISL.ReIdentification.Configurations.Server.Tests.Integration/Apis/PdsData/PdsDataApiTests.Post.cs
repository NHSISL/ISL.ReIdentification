// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.PdsData;

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.Apis
{
    public partial class PdsDataApiTests
    {
        [Fact]
        public async Task ShouldPostPdsDataAsync()
        {
            // given
            PdsData randomPdsData = CreateRandomPdsData();
            PdsData expectedPdsData = randomPdsData;

            // when 
            await apiBroker.PostPdsDataAsync(randomPdsData);

            PdsData actualPdsData =
                await apiBroker.GetPdsDataByIdAsync(randomPdsData.Id);

            // then
            actualPdsData.Should().BeEquivalentTo(expectedPdsData);
            await apiBroker.DeletePdsDataByIdAsync(actualPdsData.Id);
        }
    }
}
