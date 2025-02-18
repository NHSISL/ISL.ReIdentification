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
            actualLookup.Should().BeEquivalentTo(expectedLookup, options =>
                options.Excluding(lookup => lookup.CreatedBy)
                       .Excluding(lookup => lookup.CreatedDate)
                       .Excluding(lookup => lookup.UpdatedBy)
                       .Excluding(lookup => lookup.UpdatedDate));

            await this.apiBroker.DeleteLookupByIdAsync(actualLookup.Id);
        }
    }
}