// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    public partial class OdsDatasApiTests
    {
        [Fact(Skip = "Need to refactor tests and add other crud operations")]
        public async Task ShouldGetAllOdsDatasAsync()
        {
            // when
            var actualOdsDatas = await this.apiBroker.GetAllOdsDatasAsync();

            // then
            actualOdsDatas.Should().NotBeNull();
        }

        [Fact(Skip = "Need to refactor tests and add other crud operations")]
        public async Task ShouldGetOdsDataByIdAsync()
        {
            // given
            Guid randomOdsDataId = Guid.NewGuid();

            // when
            var actualOdsData = await this.apiBroker.GetOdsDataByIdAsync(randomOdsDataId);

            // then
            actualOdsData.Should().NotBeNull();
        }
    }
}
