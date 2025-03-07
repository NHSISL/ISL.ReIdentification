// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using RESTFulSense.Exceptions;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.PdsData;

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.Apis
{
    public partial class PdsDataApiTests
    {
        [Fact]
        public async Task ShouldDeletePdsDataAsync()
        {
            // given
            PdsData randomPdsData = await PostRandomPdsDataAsync();

            // when
            await this.apiBroker.DeletePdsDataByIdAsync(randomPdsData.Id);

            // then
            ValueTask<PdsData> getPdsDataByIdTask =
                this.apiBroker.GetPdsDataByIdAsync(randomPdsData.Id);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(getPdsDataByIdTask.AsTask);
        }
    }
}
