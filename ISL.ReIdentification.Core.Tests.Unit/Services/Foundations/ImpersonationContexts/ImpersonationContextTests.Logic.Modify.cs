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
                CreateRandomImpersonationContext(
                    randomDateTimeOffset,
                    impersonationContextId:
                    randomEntraUser.EntraUserId);

            ImpersonationContext inputImpersonationContext = randomImpersonationContext.DeepClone();
            ImpersonationContext storageImpersonationContext = randomImpersonationContext.DeepClone();
            storageImpersonationContext.UpdatedDate = storageImpersonationContext.CreatedDate;
            ImpersonationContext updatedImpersonationContext = inputImpersonationContext.DeepClone();
            ImpersonationContext expectedImpersonationContext = updatedImpersonationContext.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

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
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(inputImpersonationContext.Id),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateImpersonationContextAsync(inputImpersonationContext),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}