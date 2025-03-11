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
        public async Task ShouldGetDescendantsAsync()
        {
            // given
            OdsData parentOdsData = await PostRandomOdsDataAsync();
            List<OdsData> expectedDescendants = await PostRandomChildOdsDatasAsync(parentOdsData.OdsHierarchy);

            // when
            List<OdsData> actualDescendants = await this.apiBroker.GetDescendantsAsync(parentOdsData.Id);

            // then
            actualDescendants.Should().BeEquivalentTo(expectedDescendants);

            foreach (var descendant in expectedDescendants)
            {
                await this.apiBroker.DeleteOdsDataByIdAsync(descendant.Id);
            }

            await this.apiBroker.DeleteOdsDataByIdAsync(parentOdsData.Id);
        }
    }
}
