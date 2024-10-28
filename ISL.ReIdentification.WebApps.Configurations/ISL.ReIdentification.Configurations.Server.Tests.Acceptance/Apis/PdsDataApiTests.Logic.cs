// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.PdsDatas;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    public partial class PdsDataApiTests
    {
        [Fact]
        public async Task ShouldPostPdsDataAsync()
        {
            // given
            PdsData randomPdsData = CreateRandomPdsData();
            PdsData inputPdsData = randomPdsData;
            PdsData expectedPdsData = inputPdsData;

            // when 
            await this.apiBroker.PostPdsDataAsync(inputPdsData);

            PdsData actuaPdsData =
                await this.apiBroker.GetPdsDataByIdAsync(inputPdsData.Id);

            // then
            actuaPdsData.Should().BeEquivalentTo(expectedPdsData);
            await this.apiBroker.DeletePdsDataByIdAsync(actuaPdsData.Id);
        }

        [Fact]
        public async Task ShouldGetPdsDataByIdAsync()
        {
            // given
            PdsData randomPdsData = await PostRandomPdsDataAsync();
            PdsData expectedPdsData = randomPdsData;

            // when
            PdsData actualPdsData = await this.apiBroker.GetPdsDataByIdAsync(randomPdsData.Id);

            // then
            actualPdsData.Should().BeEquivalentTo(expectedPdsData);
            await this.apiBroker.DeletePdsDataByIdAsync(actualPdsData.Id);
        }
    }
}
