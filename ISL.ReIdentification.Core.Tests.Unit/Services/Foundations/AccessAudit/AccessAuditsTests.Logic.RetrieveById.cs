// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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
        public async Task ShouldRetrieveByIdAccessAuditAsync()
        {
            // given
            AccessAudit randomAccessAudit = CreateRandomAccessAudit();
            AccessAudit inputAccessAudit = randomAccessAudit;
            AccessAudit storageAccessAudit = inputAccessAudit.DeepClone();

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(inputAccessAudit.Id))
                    .ReturnsAsync(storageAccessAudit);

            // when
            AccessAudit actualAccessAudit =
                await this.accessAuditService.RetrieveAccessAuditByIdAsync(inputAccessAudit.Id);

            // then
            actualAccessAudit.Should().BeEquivalentTo(storageAccessAudit);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(inputAccessAudit.Id),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
