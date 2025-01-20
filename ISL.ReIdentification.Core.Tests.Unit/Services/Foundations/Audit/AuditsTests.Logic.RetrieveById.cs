// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Audits
{
    public partial class AuditTests
    {
        [Fact]
        public async Task ShouldRetrieveByIdAuditAsync()
        {
            // given
            Audit randomAudit = CreateRandomAudit();
            Audit inputAudit = randomAudit;
            Audit storageAudit = inputAudit.DeepClone();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(inputAudit.Id))
                    .ReturnsAsync(storageAudit);

            // when
            Audit actualAudit =
                await this.accessAuditService.RetrieveAuditByIdAsync(inputAudit.Id);

            // then
            actualAudit.Should().BeEquivalentTo(storageAudit);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(inputAudit.Id),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
