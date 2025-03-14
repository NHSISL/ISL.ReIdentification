// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.Lookup;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis.Lookups
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
            actualLookup.Should().BeEquivalentTo(
                modifiedLookup,
                options => options
                    .Excluding(lookup => lookup.CreatedBy)
                    .Excluding(lookup => lookup.CreatedDate)
                    .Excluding(lookup => lookup.UpdatedBy)
                    .Excluding(lookup => lookup.UpdatedDate));

            await this.apiBroker.DeleteLookupByIdAsync(actualLookup.Id);
        }
    }
}

