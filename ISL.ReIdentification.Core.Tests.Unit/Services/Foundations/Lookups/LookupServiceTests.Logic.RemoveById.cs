// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Data;
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
        public async Task ShouldRemoveLookupByIdAsync()
        {
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            Lookup randomLookup = CreateRandomLookup(randomDateTimeOffset, randomEntraUser.EntraUserId);

            Guid inputLookupId = randomLookup.Id;
            Lookup storageLookup = randomLookup;

            Lookup lookupWithDeleteAuditApplied = storageLookup.DeepClone();
            lookupWithDeleteAuditApplied.UpdatedBy = randomEntraUser.EntraUserId.ToString();
            lookupWithDeleteAuditApplied.UpdatedDate = randomDateTimeOffset;

            Lookup updatedLookup = lookupWithDeleteAuditApplied;
            Lookup deletedLookup = updatedLookup;
            Lookup expectedLookup = deletedLookup.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectLookupByIdAsync(inputLookupId))
                    .ReturnsAsync(storageLookup);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.UpdateLookupAsync(randomLookup))
                    .ReturnsAsync(updatedLookup);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.DeleteLookupAsync(updatedLookup))
                    .ReturnsAsync(deletedLookup);

            //When
            Lookup actualLookup = await this.lookupService.RemoveLookupByIdAsync(inputLookupId);

            //then
            actualLookup.Should().BeEquivalentTo(expectedLookup);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectLookupByIdAsync(inputLookupId),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateLookupAsync(randomLookup),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteLookupAsync(updatedLookup),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}