// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Lookups
{
    public partial class LookupServiceTests
    {
        [Fact]
        public async Task ShouldModifyLookupAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            Lookup randomLookup = CreateRandomModifyLookup(
                randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            Lookup inputLookup = randomLookup;
            Lookup storageLookup = inputLookup.DeepClone();
            storageLookup.UpdatedDate = randomLookup.CreatedDate;
            Lookup updatedLookup = inputLookup;
            Lookup expectedLookup = updatedLookup.DeepClone();
            Guid lookupId = inputLookup.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectLookupByIdAsync(lookupId))
                    .ReturnsAsync(storageLookup);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.UpdateLookupAsync(inputLookup))
                    .ReturnsAsync(updatedLookup);

            // when
            Lookup actualLookup =
                await this.lookupService.ModifyLookupAsync(inputLookup);

            // then
            actualLookup.Should().BeEquivalentTo(expectedLookup);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectLookupByIdAsync(inputLookup.Id),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateLookupAsync(inputLookup),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}