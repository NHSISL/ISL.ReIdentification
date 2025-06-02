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
        public async Task ShouldModifyAuditAsync()
        {
            // given
            DateTimeOffset randomDateOffset = GetRandomDateTimeOffset();

            Audit randomModifyAudit =
                CreateRandomModifyAudit(randomDateOffset);

            Audit inputAudit = randomModifyAudit.DeepClone();
            Audit storageAudit = randomModifyAudit.DeepClone();
            storageAudit.UpdatedDate = storageAudit.CreatedDate;
            Audit updatedAudit = inputAudit.DeepClone();
            Audit expectedAudit = updatedAudit.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateOffset);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(inputAudit.Id))
                    .ReturnsAsync(storageAudit);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.UpdateAuditAsync(inputAudit))
                    .ReturnsAsync(updatedAudit);

            // when
            Audit actualAudit =
                await this.auditService.ModifyAuditAsync(inputAudit);

            // then
            actualAudit.Should().BeEquivalentTo(expectedAudit);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(inputAudit.Id),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateAuditAsync(inputAudit),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
