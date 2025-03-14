// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using RESTFulSense.Exceptions;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.OdsData;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis
{
    public partial class OdsDataApiTests
    {
        [Fact]
        public async Task ShouldDeleteOdsDataAsync()
        {
            // given
            OdsData randomOdsData = await PostRandomOdsDataAsync();

            // when
            await this.apiBroker.DeleteOdsDataByIdAsync(randomOdsData.Id);

            // then
            ValueTask<OdsData> getOdsDataTask = this.apiBroker.GetOdsDataByIdAsync(randomOdsData.Id);
            await Assert.ThrowsAsync<HttpResponseNotFoundException>(getOdsDataTask.AsTask);
        }
    }
}
