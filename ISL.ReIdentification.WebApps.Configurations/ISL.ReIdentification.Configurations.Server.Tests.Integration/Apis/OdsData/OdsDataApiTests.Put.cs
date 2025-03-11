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
        public async Task ShouldPutOdsDataAsync()
        {
            // given
            OdsData randomOdsData = await PostRandomOdsDataAsync();
            OdsData modifiedOdsData = UpdateOdsDataWithRandomValues(randomOdsData);

            // when
            await this.apiBroker.PutOdsDataAsync(modifiedOdsData);
            OdsData actualOdsData = await this.apiBroker.GetOdsDataByIdAsync(randomOdsData.Id);

            // then
            actualOdsData.Should().BeEquivalentTo(modifiedOdsData);

            await this.apiBroker.DeleteOdsDataByIdAsync(randomOdsData.Id);
        }
    }
}
