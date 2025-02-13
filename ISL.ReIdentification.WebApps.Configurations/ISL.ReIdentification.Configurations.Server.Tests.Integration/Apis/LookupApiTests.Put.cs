// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.ReIdentification.Configuration.Server.Tests.Integration.Models;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis
{
    public partial class LookupApiTests
    {
        [Fact]
        public async Task ShouldPutLookupAsync()
        {
            // given
            Lookup randomLookup = await PostRandomLookupAsync();
            Lookup modifiedLookup = UpdateLookupWithRandomValues(randomLookup);

            // when
            await this.apiBroker.PutLookupAsync(modifiedLookup);
            Lookup actualLookup = await this.apiBroker.GetLookupByIdAsync(randomLookup.Id);

            // then
            actualLookup.Should().BeEquivalentTo(modifiedLookup);
            await this.apiBroker.DeleteLookupByIdAsync(actualLookup.Id);
        }
    }
}

