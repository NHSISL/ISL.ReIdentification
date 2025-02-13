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
        public async Task ShouldGetLookupByIdAsync()
        {
            // given
            Lookup randomLookup = await PostRandomLookupAsync();
            Lookup expectedLookup = randomLookup;

            // when
            Lookup actualLookup = await this.apiBroker.GetLookupByIdAsync(randomLookup.Id);

            // then
            actualLookup.Should().BeEquivalentTo(expectedLookup);
            await this.apiBroker.DeleteLookupByIdAsync(actualLookup.Id);
        }
    }
}