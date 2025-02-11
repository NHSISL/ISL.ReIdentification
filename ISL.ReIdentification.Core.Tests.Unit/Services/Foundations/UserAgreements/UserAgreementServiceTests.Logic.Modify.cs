// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldModifyUserAgreementAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            UserAgreement randomUserAgreement = CreateRandomModifyUserAgreement(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            UserAgreement inputUserAgreement = randomUserAgreement;
            UserAgreement storageUserAgreement = inputUserAgreement.DeepClone();
            storageUserAgreement.UpdatedDate = randomUserAgreement.CreatedDate;
            UserAgreement updatedUserAgreement = inputUserAgreement;
            UserAgreement expectedUserAgreement = updatedUserAgreement.DeepClone();
            Guid userAgreementId = inputUserAgreement.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectUserAgreementByIdAsync(userAgreementId))
                    .ReturnsAsync(storageUserAgreement);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.UpdateUserAgreementAsync(inputUserAgreement))
                    .ReturnsAsync(updatedUserAgreement);

            // when
            UserAgreement actualUserAgreement =
                await this.userAgreementService.ModifyUserAgreementAsync(inputUserAgreement);

            // then
            actualUserAgreement.Should().BeEquivalentTo(expectedUserAgreement);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(inputUserAgreement.Id),
                    Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.UpdateUserAgreementAsync(inputUserAgreement),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}