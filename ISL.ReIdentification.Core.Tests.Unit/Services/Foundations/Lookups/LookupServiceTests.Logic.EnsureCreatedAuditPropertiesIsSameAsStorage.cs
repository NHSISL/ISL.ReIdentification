// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Services.Foundations.Lookups;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Lookups
{
    public partial class LookupServiceTests
    {
        [Fact]
        public async Task ShouldEnsureCreatedAuditPropertiesIsSameAsStorageAsync()
        {
            // given
            Lookup inputLookup = CreateRandomLookup();
            Lookup maybeLookup = CreateRandomLookup();
            Lookup expectedLookup = inputLookup.DeepClone();
            expectedLookup.CreatedDate = maybeLookup.CreatedDate;
            expectedLookup.CreatedBy = maybeLookup.CreatedBy;

            var terminologyArtifactServiceMock = new Mock<LookupService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            // when
            Lookup actualLookup =
                await terminologyArtifactServiceMock.Object.EnsureCreatedAuditPropertiesIsSameAsStorageAsync(
                    inputLookup, maybeLookup);

            // then
            actualLookup.Should().BeEquivalentTo(expectedLookup);

            terminologyArtifactServiceMock.Verify(service =>
                service.EnsureCreatedAuditPropertiesIsSameAsStorageAsync(
                    inputLookup, maybeLookup),
                        Times.Once());

            terminologyArtifactServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}