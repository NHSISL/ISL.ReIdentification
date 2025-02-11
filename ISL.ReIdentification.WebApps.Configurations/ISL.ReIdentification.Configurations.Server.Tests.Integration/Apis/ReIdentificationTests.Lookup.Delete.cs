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
        public async Task ShouldDeleteLookupAsync()
        {
            // given
            Lookup randomLookup = await PostRandomLookupAsync();

            // when
            await this.apiBroker.DeleteLookupByIdAsync(randomLookup.Id);

            // then
            Lookup deletedLookup = await this.apiBroker.GetLookupByIdAsync(randomLookup.Id);
            deletedLookup.Should().BeNull();
        }
    }
}