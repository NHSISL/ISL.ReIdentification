// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Audits
{
    public partial class AuditTests
    {
        [Fact]
        public async Task ShouldRetrieveAllAuditsAsync()
        {
            // given
            IQueryable<Audit> randomAudits = CreateRandomAudits();
            IQueryable<Audit> storageAudits = randomAudits;
            IQueryable<Audit> expectedAudits = storageAudits;

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllAuditsAsync())
                    .ReturnsAsync(storageAudits);

            // when
            IQueryable<Audit> actualAudits = await this.auditService.RetrieveAllAuditsAsync();

            // then
            actualAudits.Should().BeEquivalentTo(expectedAudits);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllAuditsAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
