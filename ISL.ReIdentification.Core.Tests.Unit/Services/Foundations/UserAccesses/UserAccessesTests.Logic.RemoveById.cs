// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Data;
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
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            UserAccess randomUserAccess = CreateRandomUserAccess(randomDateTimeOffset, randomEntraUser.EntraUserId);
            Guid inputUserAccessId = randomUserAccess.Id;
            UserAccess storageUserAccess = randomUserAccess;
            UserAccess userAccessWithDeleteAuditApplied = storageUserAccess.DeepClone();
            userAccessWithDeleteAuditApplied.UpdatedBy = randomEntraUser.EntraUserId.ToString();
            userAccessWithDeleteAuditApplied.UpdatedDate = randomDateTimeOffset;
            UserAccess updatedUserAccess = userAccessWithDeleteAuditApplied;
            UserAccess deletedUserAccess = updatedUserAccess;
            UserAccess expectedUserAccess = deletedUserAccess.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectUserAccessByIdAsync(inputUserAccessId))
                    .ReturnsAsync(storageUserAccess);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.UpdateUserAccessAsync(randomUserAccess))
                    .ReturnsAsync(updatedUserAccess);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.DeleteUserAccessAsync(updatedUserAccess))
                    .ReturnsAsync(deletedUserAccess);

            UserAccess actualUserAccess = await this.userAccessService.RemoveUserAccessByIdAsync(inputUserAccessId);

            actualUserAccess.Should().BeEquivalentTo(expectedUserAccess);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectUserAccessByIdAsync(inputUserAccessId),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateUserAccessAsync(randomUserAccess),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteUserAccessAsync(updatedUserAccess),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
