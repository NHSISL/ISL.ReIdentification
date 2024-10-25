// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
            Guid randomId = Guid.NewGuid();
            Guid inputId = randomId;
            List<string> randomOrganisations = GetRandomStringsWithLengthOf(10);
            List<string> inputOrganisations = randomOrganisations;
            List<string> storageOrganisations = inputOrganisations.DeepClone();
            List<string> expectedOrganisations = inputOrganisations.DeepClone();

            this.userAccessServiceMock.Setup(service =>
                service.RetrieveAllOrganisationsUserHasAccessTo(inputId))
                    .ReturnsAsync(storageOrganisations);

            // when
            List<string> actualOrganisations = await this.userAccessProcessingService
                .RetrieveAllActiveOrganisationsUserHasAccessTo(inputId);

            // then
            actualOrganisations.Should().BeEquivalentTo(expectedOrganisations);

            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllOrganisationsUserHasAccessTo(inputId),
                    Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
