// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Data;
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
        public async Task ShouldRemoveAccessAuditByIdAsync()
        {
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            AccessAudit randomAccessAudit = CreateRandomAccessAudit(randomDateTimeOffset, randomEntraUser.EntraUserId);
            Guid inputAccessAuditId = randomAccessAudit.Id;
            AccessAudit storageAccessAudit = randomAccessAudit;
            AccessAudit accessAuditWithDeleteAuditApplied = storageAccessAudit.DeepClone();
            accessAuditWithDeleteAuditApplied.UpdatedBy = randomEntraUser.EntraUserId.ToString();
            accessAuditWithDeleteAuditApplied.UpdatedDate = randomDateTimeOffset;
            AccessAudit updatedAccessAudit = accessAuditWithDeleteAuditApplied;
            AccessAudit deletedAccessAudit = updatedAccessAudit;
            AccessAudit expectedAccessAudit = deletedAccessAudit.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(inputAccessAuditId))
                    .ReturnsAsync(storageAccessAudit);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.UpdateAccessAuditAsync(randomAccessAudit))
                    .ReturnsAsync(updatedAccessAudit);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.DeleteAccessAuditAsync(updatedAccessAudit))
                    .ReturnsAsync(deletedAccessAudit);

            AccessAudit actualAccessAudit = 
                await this.accessAuditService.RemoveAccessAuditByIdAsync(inputAccessAuditId);

            actualAccessAudit.Should().BeEquivalentTo(expectedAccessAudit);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(inputAccessAuditId),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.UpdateAccessAuditAsync(randomAccessAudit),
                    Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.DeleteAccessAuditAsync(updatedAccessAudit),
                    Times.Once);

            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
