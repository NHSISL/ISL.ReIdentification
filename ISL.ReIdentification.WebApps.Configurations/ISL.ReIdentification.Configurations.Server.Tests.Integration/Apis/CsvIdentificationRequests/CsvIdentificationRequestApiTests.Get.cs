// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.Lookup;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestApiTests
    {
        [Fact]
        public async Task ShouldGetAllCsvIdentificationRequestsAsync()
        {
            // when
            List<CsvIdentificationRequest> actualCsvRequests =
                await this.apiBroker.GetAllCsvIdentificationRequestsAsync();

            // then
            actualCsvRequests.Should().NotBeNull();
            actualCsvRequests.Should().NotBeEmpty();

            actualCsvRequests.Should().AllBeOfType<CsvIdentificationRequest>();
        }
    }
}