// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Audits
{
    public partial class AuditTests
    {
        [Fact]
        public async Task ShouldModifyAuditAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            Audit randomAudit =
                CreateRandomModifyAudit(randomDateTimeOffset, randomEntraUser.EntraUserId);

            Audit inputAudit = randomAudit;
            Audit storageAudit = inputAudit.DeepClone();
            storageAudit.UpdatedDate = randomAudit.CreatedDate;
            Audit updatedAudit = inputAudit;
            Audit expectedAudit = updatedAudit.DeepClone();
            Guid auditId = inputAudit.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(auditId))
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
                    Times.Exactly(2));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(inputAudit.Id),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateAuditAsync(inputAudit),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
