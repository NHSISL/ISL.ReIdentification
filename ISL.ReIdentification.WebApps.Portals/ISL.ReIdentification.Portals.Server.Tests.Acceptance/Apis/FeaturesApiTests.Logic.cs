// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    public partial class FeaturesApiTests
    {

        [Fact]
        public async Task ShouldGetFeaturesAsync()
        {
            // Given
            string expectedResult =
                "[" +
                "\r\n  \"Configuration\"," +
                "\r\n  \"UserAccess\"," +
                "\r\n  \"DelegatedUserAccess\"," +
                "\r\n  \"Ods\"," +
                "\r\n  \"Pds\"," +
                "\r\n  \"ReportReidentify\"," +
                "\r\n  \"SinglePatientReidentify\"," +
                "\r\n  \"CsvReidentify\"," +
                "\r\n  \"Projects\"," +
                "\r\n  \"CsvWorklist\"" +
                "\r\n]";
            // When
            string actualResult = await this.apiBroker.GetFeaturesAsync();

            // Then
            actualResult.Should().BeEquivalentTo(expectedResult);

        }
    }
}