// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Processings.UserAccesses
{
    public partial class UserAccessProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllUserAccessesAsync()
        {
            // given
            List<UserAccess> randomUserAccesses = CreateRandomUserAccesses();
            IQueryable<UserAccess> inputUserAccesses = randomUserAccesses.AsQueryable();
            IQueryable<UserAccess> storageUserAccesses = inputUserAccesses.DeepClone();
            IQueryable<UserAccess> expectedUserAccesses = inputUserAccesses.DeepClone();

            this.userAccessServiceMock.Setup(service =>
                service.RetrieveAllUserAccessesAsync())
                    .ReturnsAsync(storageUserAccesses);

            // when
            IQueryable<UserAccess> actualUserAccess = await this.userAccessProcessingService
                .RetrieveAllUserAccessesAsync();

            // then
            actualUserAccess.Should().BeEquivalentTo(expectedUserAccesses);

            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllUserAccessesAsync(),
                    Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
