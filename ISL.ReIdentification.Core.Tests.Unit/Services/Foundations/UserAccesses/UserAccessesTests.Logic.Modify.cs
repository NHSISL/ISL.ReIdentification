// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldModifyUserAccessAsync()
        {
            // given
            DateTimeOffset randomDateOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();

            UserAccess randomModifyUserAccess =
                CreateRandomModifyUserAccess(randomDateOffset, randomUserId);

            UserAccess inputUserAccess = randomModifyUserAccess.DeepClone();
            UserAccess storageUserAccess = randomModifyUserAccess.DeepClone();
            storageUserAccess.UpdatedDate = storageUserAccess.CreatedDate;
            UserAccess updatedUserAccess = inputUserAccess.DeepClone();
            UserAccess expectedUserAccess = updatedUserAccess.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateOffset);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectUserAccessByIdAsync(inputUserAccess.Id))
                    .ReturnsAsync(storageUserAccess);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.UpdateUserAccessAsync(inputUserAccess))
                    .ReturnsAsync(updatedUserAccess);

            // when
            UserAccess actualUserAccess =
                await this.userAccessService.ModifyUserAccessAsync(inputUserAccess);

            // then
            actualUserAccess.Should().BeEquivalentTo(expectedUserAccess);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectUserAccessByIdAsync(inputUserAccess.Id),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateUserAccessAsync(inputUserAccess),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
