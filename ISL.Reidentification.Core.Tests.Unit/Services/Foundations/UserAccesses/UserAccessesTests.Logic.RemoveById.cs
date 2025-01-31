// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAccesses
{
    public partial class UserAccessesTests
    {
        [Fact]
        public async Task ShouldRemoveUserAccessByIdAsync()
        {
            // given
            DateTimeOffset randomDateOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            UserAccess randomUserAccess = CreateRandomUserAccess();
            UserAccess inputUserAccess = randomUserAccess;
            Guid inputUserAccessId = inputUserAccess.Id;
            UserAccess storageUserAccess = inputUserAccess.DeepClone();
            UserAccess updatedUserAccess = storageUserAccess.DeepClone();
            updatedUserAccess.UpdatedBy = randomEntraUser.EntraUserId.ToString();
            updatedUserAccess.UpdatedDate = randomDateOffset;
            UserAccess deletedUserAccess = updatedUserAccess.DeepClone();
            UserAccess expectedUserAccess = deletedUserAccess.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectUserAccessByIdAsync(inputUserAccessId))
                    .ReturnsAsync(storageUserAccess);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.UpdateUserAccessAsync(It.Is(SameUserAccessAs(updatedUserAccess))))
                    .ReturnsAsync(updatedUserAccess);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.DeleteUserAccessAsync(It.Is(SameUserAccessAs(updatedUserAccess))))
                    .ReturnsAsync(deletedUserAccess);

            // when
            UserAccess actualUserAccess =
                await this.userAccessService.RemoveUserAccessByIdAsync(inputUserAccessId);

            // then
            actualUserAccess.Should().BeEquivalentTo(expectedUserAccess);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectUserAccessByIdAsync(inputUserAccessId),
                    Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateUserAccessAsync(It.Is(SameUserAccessAs(updatedUserAccess))),
                    Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteUserAccessAsync(It.Is(SameUserAccessAs(updatedUserAccess))),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
