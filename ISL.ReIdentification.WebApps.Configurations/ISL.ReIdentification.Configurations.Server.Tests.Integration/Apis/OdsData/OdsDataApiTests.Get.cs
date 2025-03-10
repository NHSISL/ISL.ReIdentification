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
        public async Task ShouldGetAllOdsDataAsync()
        {
            // given
            List<OdsData> expectedOdsDatas = await PostRandomOdsDatasAsync();

            // when
            List<OdsData> actualOdsDatas = await this.apiBroker.GetAllOdsDatasAsync();

            // then
            actualOdsDatas.Should().Contain(expectedOdsDatas);
        }
    }
}
