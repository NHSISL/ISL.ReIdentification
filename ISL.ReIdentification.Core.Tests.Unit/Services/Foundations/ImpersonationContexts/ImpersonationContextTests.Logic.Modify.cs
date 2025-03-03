// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.ImpersonationContexts
{
    public partial class ImpersonationContextsTests
    {
        [Fact]
        public async Task ShouldModifyImpersonationContextAsync()
        {
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            ImpersonationContext randomImpersonationContext =
                CreateRandomModifyImpersonationContext(
                    randomDateTimeOffset,
                    impersonationContextId:
                    randomEntraUser.EntraUserId);

            ImpersonationContext inputImpersonationContext = randomImpersonationContext;
            ImpersonationContext storageImpersonationContext = inputImpersonationContext.DeepClone();
            storageImpersonationContext.UpdatedDate = randomImpersonationContext.CreatedDate;
            ImpersonationContext updatedImpersonationContext = inputImpersonationContext;
            ImpersonationContext expectedImpersonationContext = updatedImpersonationContext.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectImpersonationContextByIdAsync(inputImpersonationContext.Id))
                    .ReturnsAsync(storageImpersonationContext);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.UpdateImpersonationContextAsync(inputImpersonationContext))
                    .ReturnsAsync(updatedImpersonationContext);

            //when
            ImpersonationContext actualImpersonationContext =
                await this.impersonationContextService.ModifyImpersonationContextAsync(inputImpersonationContext);

            //then
            actualImpersonationContext.Should().BeEquivalentTo(expectedImpersonationContext);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(inputImpersonationContext.Id),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateImpersonationContextAsync(inputImpersonationContext),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}