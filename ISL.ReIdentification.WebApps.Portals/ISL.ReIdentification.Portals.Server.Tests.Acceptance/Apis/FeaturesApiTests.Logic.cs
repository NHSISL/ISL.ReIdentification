// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    public partial class FeaturesApiTests
    {

        [Fact(Skip = "Need To Check")]
        public async Task ShouldGetFeaturesAsync()
        {
            // Given
            string expectedResult = "[\"Configuration\",\"Test\"]";

            // When
            string actualResult = await this.apiBroker.GetFeaturesAsync();

            // Then
            actualResult.Should().BeEquivalentTo(expectedResult);

        }
    }
}