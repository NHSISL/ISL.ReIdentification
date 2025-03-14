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
        public async Task ShouldGetAncestorsAsync()
        {
            // given
            OdsData parentOdsData = await PostRandomOdsDataAsync();
            List<OdsData> childOdsDatas = await PostRandomChildOdsDatasAsync(parentOdsData.OdsHierarchy);
            OdsData childOdsData = childOdsDatas.First();

            // when
            List<OdsData> actualAncestors = await this.apiBroker.GetAncestorsAsync(childOdsData.Id);

            // then
            actualAncestors.Should().ContainSingle(a => a.Id == parentOdsData.Id);

            // Cleanup
            foreach (var child in childOdsDatas)
            {
                await this.apiBroker.DeleteOdsDataByIdAsync(child.Id);
            }

            await this.apiBroker.DeleteOdsDataByIdAsync(parentOdsData.Id);
        }
    }
}
