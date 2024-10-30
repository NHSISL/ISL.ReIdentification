// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.OdsDatas;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    public partial class OdsDataApiTests
    {
        [Fact]
        public async Task ShouldPostOdsDataAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            OdsData inputOdsData = randomOdsData;
            OdsData expectedOdsData = inputOdsData;

            // when 
            await this.apiBroker.PostOdsDataAsync(inputOdsData);

            OdsData actualOdsData =
                await this.apiBroker.GetOdsDataByIdAsync(inputOdsData.Id);

            // then
            actualOdsData.Should().BeEquivalentTo(expectedOdsData);
            await this.apiBroker.DeleteOdsDataByIdAsync(actualOdsData.Id);
        }

        [Fact(Skip = "Need to refactor tests and add other crud operations")]
        public async Task ShouldGetAllOdsDatasAsync()
        {
            // when
            var actualOdsDatas = await this.apiBroker.GetAllOdsDatasAsync();

            // then
            actualOdsDatas.Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldGetOdsDataByIdAsync()
        {
            // given
            OdsData randomOdsData = await PostRandomOdsDataAsync();
            OdsData expectedOdsData = randomOdsData;

            // when
            OdsData actualOdsData = await this.apiBroker.GetOdsDataByIdAsync(randomOdsData.Id);

            // then
            actualOdsData.Should().BeEquivalentTo(expectedOdsData);
            await this.apiBroker.DeleteOdsDataByIdAsync(actualOdsData.Id);
        }
    }
}
