// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAgreements
{
    public partial class UserAgreementServiceTests
    {
        [Fact]
        public async Task ShouldRemoveUserAgreementByIdAsync()
        {
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            UserAgreement randomUserAgreement = 
                CreateRandomUserAgreement(randomDateTimeOffset, randomEntraUser.EntraUserId);

            Guid inputUserAgreementId = randomUserAgreement.Id;
            UserAgreement storageUserAgreement = randomUserAgreement;
            UserAgreement userAgreementWithDeleteAuditApplied = storageUserAgreement.DeepClone();
            userAgreementWithDeleteAuditApplied.UpdatedBy = randomEntraUser.EntraUserId.ToString();
            userAgreementWithDeleteAuditApplied.UpdatedDate = randomDateTimeOffset;
            UserAgreement updatedUserAgreement = userAgreementWithDeleteAuditApplied;
            UserAgreement deletedUserAgreement = updatedUserAgreement;
            UserAgreement expectedUserAgreement = deletedUserAgreement.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectUserAgreementByIdAsync(inputUserAgreementId))
                    .ReturnsAsync(storageUserAgreement);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.UpdateUserAgreementAsync(randomUserAgreement))
                    .ReturnsAsync(updatedUserAgreement);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.DeleteUserAgreementAsync(updatedUserAgreement))
                    .ReturnsAsync(deletedUserAgreement);

            //when
            UserAgreement actualUserAgreement = await this.userAgreementService.RemoveUserAgreementByIdAsync(inputUserAgreementId);

            //then
            actualUserAgreement.Should().BeEquivalentTo(expectedUserAgreement);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(inputUserAgreementId),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.UpdateUserAgreementAsync(randomUserAgreement),
                    Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.DeleteUserAgreementAsync(updatedUserAgreement),
                    Times.Once);

            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}