// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.OdsData;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis
{
    public partial class OdsDataApiTests
    {
        [Fact]
        public async Task ShouldPostOdsDataAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            OdsData expectedOdsData = randomOdsData;

            // when
            await this.apiBroker.PostOdsDataAsync(randomOdsData);
            OdsData actualOdsData = await this.apiBroker.GetOdsDataByIdAsync(randomOdsData.Id);

            // then
            actualOdsData.Should().BeEquivalentTo(expectedOdsData);
            await this.apiBroker.DeleteOdsDataByIdAsync(randomOdsData.Id);
        }
    }
}
