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
        public async Task ShouldAddImpersonationContextAsync()
        {
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            ImpersonationContext randomImpersonationContext =
                CreateRandomImpersonationContext(
                    randomDateTimeOffset,
                    impersonationContextId: randomEntraUser.EntraUserId);

            ImpersonationContext inputImpersonationContext = randomImpersonationContext;
            ImpersonationContext storageImpersonationContext = inputImpersonationContext;
            ImpersonationContext expectedImpersonationContext = inputImpersonationContext.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.InsertImpersonationContextAsync(inputImpersonationContext))
                    .ReturnsAsync(storageImpersonationContext);

            //when
            ImpersonationContext actualImpersonationContext =
                await this.impersonationContextService.AddImpersonationContextAsync(inputImpersonationContext);

            //then
            actualImpersonationContext.Should().BeEquivalentTo(expectedImpersonationContext);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertImpersonationContextAsync(inputImpersonationContext),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}