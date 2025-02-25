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
            OdsData randomOdsData = await PostRandomOdsDataAsync();

            // when
            List<OdsData> actualAncestors = await this.apiBroker.GetAncestorsAsync(randomOdsData.Id);

            // then
            actualAncestors.Should().NotBeNull();
        }
    }
}
