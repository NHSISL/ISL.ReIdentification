// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.Audit;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis.Audits
{
    public partial class AuditsApiTests
    {
        [Fact]
        public async Task ShouldGetAllAuditsAsync()
        {
            // Given
            List<Audit> expectedAudits = await PostRandomAuditsAsync();

            // When
            List<Audit> actualAudits = await this.apiBroker.GetAllAuditsAsync();

            // Then
            actualAudits.Should().NotBeNull();

            foreach (Audit expectedAudit in expectedAudits)
            {
                Audit actualAudit = actualAudits
                    .Single(audit => audit.Id == expectedAudit.Id);

                actualAudit.Should().BeEquivalentTo(
                    expectedAudit,
                    options => options
                        .Excluding(audit => audit.CreatedBy)
                        .Excluding(audit => audit.CreatedDate)
                        .Excluding(audit => audit.UpdatedBy)
                        .Excluding(audit => audit.UpdatedDate));
            }

            foreach (var audit in expectedAudits)
            {
                await this.apiBroker.DeleteAuditByIdAsync(audit.Id);
            }
        }
    }
}