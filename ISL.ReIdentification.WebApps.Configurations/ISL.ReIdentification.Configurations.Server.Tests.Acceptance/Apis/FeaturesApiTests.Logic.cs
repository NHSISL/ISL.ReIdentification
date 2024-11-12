// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    public partial class FeaturesApiTests
    {
        [Fact]
        public async Task ShouldGetFeaturesAsync()
        {
            // Given
            List<string> expectedResult = new List<string>
            {
                "Configuration",
                "UserAccess",
                "DelegatedUserAccess",
                "Ods",
                "Pds"
            };

            // When
            string[] actualResult = await this.apiBroker.GetFeaturesAsync();

            // Then
            actualResult.Should().BeEquivalentTo(expectedResult.ToArray());
        }
    }
}