// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAccesses
{
    public partial class UserAccessesTests
    {
        [Fact]
        public async Task ShouldRetrieveAllUserAccessesAsync()
        {
            // given
            List<UserAccess> randomUserAccesses = CreateRandomUserAccesses();
            IQueryable<UserAccess> storageUserAccesses = randomUserAccesses.AsQueryable();
            IQueryable<UserAccess> expectedUserAccesses = storageUserAccesses;

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllUserAccessesAsync())
                    .ReturnsAsync(storageUserAccesses);

            // when
            IQueryable<UserAccess> actualUserAccesses = await this.userAccessService.RetrieveAllUserAccessesAsync();

            // then
            actualUserAccesses.Should().BeEquivalentTo(expectedUserAccesses);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllUserAccessesAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
