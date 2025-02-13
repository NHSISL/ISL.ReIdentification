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
        public async Task ShouldAddUserAgreementAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            UserAgreement randomUserAgreement = CreateRandomUserAgreement(
                randomDateTimeOffset, userId: randomEntraUser.EntraUserId);

            UserAgreement inputUserAgreement = randomUserAgreement;
            UserAgreement storageUserAgreement = inputUserAgreement;
            UserAgreement expectedUserAgreement = storageUserAgreement.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.InsertUserAgreementAsync(inputUserAgreement))
                    .ReturnsAsync(storageUserAgreement);

            // when
            UserAgreement actualUserAgreement = await this.userAgreementService
                .AddUserAgreementAsync(inputUserAgreement);

            // then
            actualUserAgreement.Should().BeEquivalentTo(expectedUserAgreement);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(inputUserAgreement),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}