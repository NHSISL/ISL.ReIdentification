// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------


using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.Accesses;
using Xunit;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Apis
{
    public partial class ReIdentificationTests
    {
        [Fact]
        public async Task ShouldProcessCsvIdentificationRequestAsync()
        {
            // Given
            // SETUP SOME KNOWN ACCESS REQUEST WITH DATA THAT WE KNOW EXISTS?
            AccessRequest inputAccessRequest = new AccessRequest();

            // When
            AccessRequest actualAccessRequest = await this.apiBroker.PostCsvIdentificationRequestAsync(inputAccessRequest);

            // Then
            Assert.NotNull(actualAccessRequest);
        }
    }
}
