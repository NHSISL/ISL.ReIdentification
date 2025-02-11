// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.ReIdentification.Configuration.Server.Tests.Integration.Models;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis
{
    public partial class ReIdentificationTests
    {
        [Fact]
        public async Task ShouldPostLookupAsync()
        {
            // given
            Lookup randomLookup = CreateRandomLookup();
            Lookup expectedLookup = randomLookup;

            // when 
            await this.apiBroker.PostLookupAsync(randomLookup);

            Lookup actualLookup =
                await this.apiBroker.GetLookupByIdAsync(randomLookup.Id);

            // then
            actualLookup.Should().BeEquivalentTo(expectedLookup);
            await this.apiBroker.DeleteLookupByIdAsync(actualLookup.Id);
        }
    }
}