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
        public async Task ShouldAddAccessAuditAsync()
        {
            // given
            DateTimeOffset randomDateOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            AccessAudit randomAccessAudit =
                CreateRandomAccessAudit(randomDateOffset, userId: randomEntraUser.EntraUserId);

            AccessAudit inputAccessAudit = randomAccessAudit;
            AccessAudit storageAccessAudit = inputAccessAudit.DeepClone();
            AccessAudit expectedAccessAudit = inputAccessAudit.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.InsertAccessAuditAsync(inputAccessAudit))
                    .ReturnsAsync(storageAccessAudit);

            // when
            AccessAudit actualAccessAudit = await this.accessAuditService.AddAccessAuditAsync(inputAccessAudit);

            // then
            actualAccessAudit.Should().BeEquivalentTo(expectedAccessAudit);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertAccessAuditAsync(inputAccessAudit),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
