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
        public async Task ShouldRemoveAuditByIdAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            Audit randomAudit = CreateRandomAudit(randomDateTimeOffset, randomEntraUser.EntraUserId);
            Guid inputAuditId = randomAudit.Id;
            Audit storageAudit = randomAudit;
            Audit ingestionTrackingWithDeleteAuditApplied = storageAudit.DeepClone();
            ingestionTrackingWithDeleteAuditApplied.UpdatedBy = randomEntraUser.EntraUserId.ToString();
            ingestionTrackingWithDeleteAuditApplied.UpdatedDate = randomDateTimeOffset;
            Audit updatedAudit = storageAudit;
            Audit deletedAudit = updatedAudit;
            Audit expectedAudit = deletedAudit.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(inputAuditId))
                    .ReturnsAsync(storageAudit);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.UpdateAuditAsync(randomAudit))
                    .ReturnsAsync(updatedAudit);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.DeleteAuditAsync(updatedAudit))
                    .ReturnsAsync(deletedAudit);

            // when
            Audit actualAudit = await this.auditService.RemoveAuditByIdAsync(inputAuditId);

            // then
            actualAudit.Should().BeEquivalentTo(expectedAudit);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(inputAuditId),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateAuditAsync(randomAudit),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteAuditAsync(updatedAudit),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
