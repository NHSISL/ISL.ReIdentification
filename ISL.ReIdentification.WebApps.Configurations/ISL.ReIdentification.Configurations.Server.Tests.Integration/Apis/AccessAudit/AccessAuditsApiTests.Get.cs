// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.AccessAudit;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis
{
    public partial class AccessAuditsApiTests
    {
        [Fact]
        public async Task ShouldGetAllAccessAuditsAsync()
        {
            // Given
            List<AccessAudit> expectedAccessAudits = await PostRandomAccessAuditsAsync();

            // When
            List<AccessAudit> actualAccessAudits = await this.apiBroker.GetAllAccessAuditsAsync();

            // Then
            actualAccessAudits.Should().NotBeNull();

            foreach (AccessAudit expectedAudit in expectedAccessAudits)
            {
                AccessAudit actualAudit = actualAccessAudits
                    .Single(audit => audit.Id == expectedAudit.Id);

                actualAudit.Should().BeEquivalentTo(
                    expectedAudit,
                    options => options
                        .Excluding(audit => audit.CreatedBy)
                        .Excluding(audit => audit.CreatedDate)
                        .Excluding(audit => audit.UpdatedBy)
                        .Excluding(audit => audit.UpdatedDate));
            }

            foreach (var accessAudit in expectedAccessAudits)
            {
                await this.apiBroker.DeleteAccessAuditByIdAsync(accessAudit.Id);
            }
        }
    }
}