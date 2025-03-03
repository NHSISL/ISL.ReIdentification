// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.AccessAudits
{
    public partial class AccessAuditTests
    {
        [Fact]
        public async Task ShouldModifyAccessAuditAsync()
        {
            // given
            DateTimeOffset randomDateOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            AccessAudit randomModifyAccessAudit = CreateRandomModifyAccessAudit(
                dateTimeOffset: randomDateOffset,
                userId: randomEntraUser.EntraUserId);

            AccessAudit inputAccessAudit = randomModifyAccessAudit.DeepClone();
            AccessAudit storageAccessAudit = randomModifyAccessAudit.DeepClone();
            storageAccessAudit.UpdatedDate = storageAccessAudit.CreatedDate;
            AccessAudit updatedAccessAudit = inputAccessAudit.DeepClone();
            AccessAudit expectedAccessAudit = updatedAccessAudit.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(inputAccessAudit.Id))
                    .ReturnsAsync(storageAccessAudit);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.UpdateAccessAuditAsync(inputAccessAudit))
                    .ReturnsAsync(updatedAccessAudit);

            // when
            AccessAudit actualAccessAudit =
                await this.accessAuditService.ModifyAccessAuditAsync(inputAccessAudit);

            // then
            actualAccessAudit.Should().BeEquivalentTo(expectedAccessAudit);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(inputAccessAudit.Id),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateAccessAuditAsync(inputAccessAudit),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
