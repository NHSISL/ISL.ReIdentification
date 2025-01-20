// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldRemoveAuditByIdAsync()
        {
            // given
            Audit randomAudit = CreateRandomAudit();
            Audit inputAudit = randomAudit;
            Guid inputAuditId = inputAudit.Id;
            Audit storageAudit = inputAudit;
            Audit deletedAudit = inputAudit;
            Audit expectedAudit = deletedAudit.DeepClone();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(inputAuditId))
                    .ReturnsAsync(storageAudit);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.DeleteAuditAsync(storageAudit))
                    .ReturnsAsync(deletedAudit);

            // when
            Audit actualAudit =
                await this.accessAuditService.RemoveAuditByIdAsync(inputAuditId);

            // then
            actualAudit.Should().BeEquivalentTo(expectedAudit);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(inputAuditId),
                    Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteAuditAsync(inputAudit),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
