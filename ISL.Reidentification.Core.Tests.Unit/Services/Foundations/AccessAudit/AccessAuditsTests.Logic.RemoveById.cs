// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.AccessAudits
{
    public partial class AccessAuditTests
    {
        [Fact]
        public async Task ShouldRemoveAccessAuditByIdAsync()
        {
            // given
            AccessAudit randomAccessAudit = CreateRandomAccessAudit();
            AccessAudit inputAccessAudit = randomAccessAudit;
            Guid inputAccessAuditId = inputAccessAudit.Id;
            AccessAudit storageAccessAudit = inputAccessAudit;
            AccessAudit deletedAccessAudit = inputAccessAudit;
            AccessAudit expectedAccessAudit = deletedAccessAudit.DeepClone();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(inputAccessAuditId))
                    .ReturnsAsync(storageAccessAudit);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.DeleteAccessAuditAsync(storageAccessAudit))
                    .ReturnsAsync(deletedAccessAudit);

            // when
            AccessAudit actualAccessAudit =
                await this.accessAuditService.RemoveAccessAuditByIdAsync(inputAccessAuditId);

            // then
            actualAccessAudit.Should().BeEquivalentTo(expectedAccessAudit);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(inputAccessAuditId),
                    Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteAccessAuditAsync(inputAccessAudit),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
