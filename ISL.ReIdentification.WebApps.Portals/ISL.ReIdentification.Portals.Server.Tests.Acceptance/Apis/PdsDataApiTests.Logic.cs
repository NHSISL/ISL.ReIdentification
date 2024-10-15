// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    public partial class PdsDataApiTests
    {
        [Fact(Skip = "Need to refactor tests and add other crud operations")]
        public async Task ShouldGetAllPdsDataAsync()
        {
            // when
            var actualPdsDatas = await this.apiBroker.GetAllPdsDataAsync();

            // then
            actualPdsDatas.Should().NotBeNull();
        }

        [Fact(Skip = "Need to refactor tests and add other crud operations")]
        public async Task ShouldGetPdsDataByIdAsync()
        {
            // given
            Guid randomPdsDataId = Guid.NewGuid();

            // when
            var actualPdsData = await this.apiBroker.GetOdsDataByIdAsync(randomPdsDataId);

            // then
            actualPdsData.Should().NotBeNull();
        }
    }
}
