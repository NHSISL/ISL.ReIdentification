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
        public async Task ShouldGetChildrenAsync()
        {
            // given
            OdsData parentOdsData = await PostRandomOdsDataAsync();
            List<OdsData> expectedChildren = await PostRandomChildOdsDatasAsync(parentOdsData.OdsHierarchy);

            // when
            List<OdsData> actualChildren = await this.apiBroker.GetChildrenAsync(parentOdsData.Id);

            // then
            actualChildren.Should().BeEquivalentTo(expectedChildren);

            foreach (var child in expectedChildren)
            {
                await this.apiBroker.DeleteOdsDataByIdAsync(child.Id);
            }

            await this.apiBroker.DeleteOdsDataByIdAsync(parentOdsData.Id);
        }
    }
}
