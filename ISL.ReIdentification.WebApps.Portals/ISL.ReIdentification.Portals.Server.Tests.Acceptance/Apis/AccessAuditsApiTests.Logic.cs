// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.AccessAudits;
using RESTFulSense.Exceptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    public partial class AccessAuditsApiTests
    {
        [Fact]
        public async Task ShouldPostAccessAuditAsync()
        {
            // given
            AccessAudit randomAccessAudit = CreateRandomAccessAudit();
            AccessAudit inputAccessAudit = randomAccessAudit;
            AccessAudit expectedAccessAudit = inputAccessAudit;

            // when 
            await apiBroker.PostAccessAuditAsync(inputAccessAudit);

            AccessAudit actualAccessAudit =
                await apiBroker.GetAccessAuditByIdAsync(inputAccessAudit.Id);

            // then
            actualAccessAudit.Should().BeEquivalentTo(expectedAccessAudit);
            await apiBroker.DeleteAccessAuditByIdAsync(actualAccessAudit.Id);
        }

        [Fact]
        public async Task ShouldGetAllAccessAuditsAsync()
        {
            // given
            List<AccessAudit> randomAccessAudits = await PostRandomAccessAuditsAsync();
            List<AccessAudit> expectedAccessAudits = randomAccessAudits;

            // when
            var actualAccessAudits = await apiBroker.GetAllAccessAuditsAsync();

            // then
            foreach (AccessAudit expectedAccessAudit in expectedAccessAudits)
            {
                AccessAudit actualAccessAudit = actualAccessAudits
                    .Single(actualAccessAudit => actualAccessAudit.Id == expectedAccessAudit.Id);

                actualAccessAudit.Should().BeEquivalentTo(expectedAccessAudit);
                await apiBroker.DeleteAccessAuditByIdAsync(actualAccessAudit.Id);
            }
        }

        [Fact]
        public async Task ShouldGetAccessAuditByIdAsync()
        {
            // given
            AccessAudit randomAccessAudit = await PostRandomAccessAuditAsync();
            AccessAudit expectedAccessAudit = randomAccessAudit;

            // when
            var actualAccessAudit = await apiBroker.GetAccessAuditByIdAsync(randomAccessAudit.Id);

            // then
            actualAccessAudit.Should().BeEquivalentTo(expectedAccessAudit);
            await apiBroker.DeleteAccessAuditByIdAsync(actualAccessAudit.Id);
        }

        [Fact]
        public async Task ShouldPutAccessAuditAsync()
        {
            // given
            AccessAudit randomAccessAudit = await PostRandomAccessAuditAsync();
            AccessAudit modifiedAccessAudit = UpdateAccessAuditWithRandomValues(randomAccessAudit);

            // when
            await apiBroker.PutAccessAuditAsync(modifiedAccessAudit);
            var actualAccessAudit = await apiBroker.GetAccessAuditByIdAsync(randomAccessAudit.Id);

            // then
            actualAccessAudit.Should().BeEquivalentTo(modifiedAccessAudit);
            await apiBroker.DeleteAccessAuditByIdAsync(actualAccessAudit.Id);
        }

        [Fact]
        public async Task ShouldDeleteAccessAuditAsync()
        {
            // given
            AccessAudit randomAccessAudit = await PostRandomAccessAuditAsync();
            AccessAudit inputAccessAudit = randomAccessAudit;
            AccessAudit expectedAccessAudit = inputAccessAudit;

            // when
            AccessAudit deletedAccessAudit =
                await apiBroker.DeleteAccessAuditByIdAsync(inputAccessAudit.Id);

            ValueTask<AccessAudit> getAccessAuditbyIdTask =
                apiBroker.GetAccessAuditByIdAsync(inputAccessAudit.Id);

            // then
            deletedAccessAudit.Should().BeEquivalentTo(expectedAccessAudit);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(
                testCode: getAccessAuditbyIdTask.AsTask);
        }
    }
}
