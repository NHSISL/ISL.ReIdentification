// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAccesses
{
    public partial class UserAccessesTests
    {
        [Fact]
        public async Task ShouldRetrieveByIdUserAccessAsync()
        {
            // given
            UserAccess randomUserAccess = CreateRandomUserAccess();
            UserAccess inputUserAccess = randomUserAccess;
            UserAccess storageUserAccess = inputUserAccess.DeepClone();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectUserAccessByIdAsync(inputUserAccess.Id))
                    .ReturnsAsync(storageUserAccess);

            // when
            UserAccess actualUserAccess = await this.userAccessService.RetrieveUserAccessByIdAsync(inputUserAccess.Id);

            // then
            actualUserAccess.Should().BeEquivalentTo(storageUserAccess);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectUserAccessByIdAsync(inputUserAccess.Id),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
