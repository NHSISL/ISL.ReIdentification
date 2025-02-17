// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configuration.Server.Tests.Integration.Models;
using RESTFulSense.Exceptions;

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
            await Assert.ThrowsAsync<HttpResponseNotFoundException>(
                async () => await this.apiBroker.GetLookupByIdAsync(randomLookup.Id));
        }
    }
}