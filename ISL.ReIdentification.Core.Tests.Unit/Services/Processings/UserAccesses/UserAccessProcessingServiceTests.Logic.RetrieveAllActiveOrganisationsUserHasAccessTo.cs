// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Processings.UserAccesses
{
    public partial class UserAccessProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllActiveOrganisationsUserHasAccessToAsync()
        {
            // given
            string randomEntraId = GetRandomStringWithLengthOf(255);
            string inputEntraId = randomEntraId;
            List<string> randomOrganisations = GetRandomStringsWithLengthOf(10);
            List<string> inputOrganisations = randomOrganisations;
            List<string> storageOrganisations = inputOrganisations.DeepClone();
            List<string> expectedOrganisations = inputOrganisations.DeepClone();

            this.userAccessServiceMock.Setup(service =>
                service.RetrieveAllActiveOrganisationsUserHasAccessToAsync(inputEntraId))
                    .ReturnsAsync(storageOrganisations);

            // when
            List<string> actualOrganisations = await this.userAccessProcessingService
                .RetrieveAllActiveOrganisationsUserHasAccessToAsync(inputEntraId);

            // then
            actualOrganisations.Should().BeEquivalentTo(expectedOrganisations);

            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllActiveOrganisationsUserHasAccessToAsync(inputEntraId),
                    Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
