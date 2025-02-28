// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis
{
    public partial class LookupApiTests
    {
        [Fact]
        public async Task ShouldDeleteLookupAsync()
        {
            // given
            Lookup randomLookup = await PostRandomLookupAsync();

            // when
            await this.apiBroker.DeleteLookupByIdAsync(randomLookup.Id);

            // then
            ValueTask<Lookup> getLookupByIdTask = this.apiBroker.GetLookupByIdAsync(randomLookup.Id);
            await Assert.ThrowsAsync<HttpResponseNotFoundException>(getLookupByIdTask.AsTask);
        }
    }
}