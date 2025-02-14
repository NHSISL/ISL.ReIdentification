// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAgreements
{
    public partial class UserAgreementServiceTests
    {
        [Fact]
        public async Task ShouldRemoveUserAgreementByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputUserAgreementId = randomId;
            UserAgreement randomUserAgreement = CreateRandomUserAgreement();
            UserAgreement storageUserAgreement = randomUserAgreement;
            UserAgreement expectedInputUserAgreement = storageUserAgreement;
            UserAgreement deletedUserAgreement = expectedInputUserAgreement;
            UserAgreement expectedUserAgreement = deletedUserAgreement.DeepClone();

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectUserAgreementByIdAsync(inputUserAgreementId))
                    .ReturnsAsync(storageUserAgreement);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.DeleteUserAgreementAsync(expectedInputUserAgreement))
                    .ReturnsAsync(deletedUserAgreement);

            // when
            UserAgreement actualUserAgreement = await this.userAgreementService
                .RemoveUserAgreementByIdAsync(inputUserAgreementId);

            // then
            actualUserAgreement.Should().BeEquivalentTo(expectedUserAgreement);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(inputUserAgreementId),
                    Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.DeleteUserAgreementAsync(expectedInputUserAgreement),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}