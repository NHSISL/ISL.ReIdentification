﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.AccessAudits
{
    public partial class AccessAuditTests
    {
        [Fact]
        public async Task ShouldRetrieveAllAccessAuditsAsync()
        {
            // given
            IQueryable<AccessAudit> randomAccessAudits = CreateRandomAccessAudits();
            IQueryable<AccessAudit> storageAccessAudits = randomAccessAudits;
            IQueryable<AccessAudit> expectedAccessAudits = storageAccessAudits;

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllAccessAuditsAsync())
                    .ReturnsAsync(storageAccessAudits);

            // when
            IQueryable<AccessAudit> actualAccessAudits = await this.accessAuditService.RetrieveAllAccessAuditsAsync();

            // then
            actualAccessAudits.Should().BeEquivalentTo(expectedAccessAudits);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllAccessAuditsAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
