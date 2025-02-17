// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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
        public async Task ShouldRetrieveUserAgreementByIdAsync()
        {
            // given
            UserAgreement randomUserAgreement = CreateRandomUserAgreement();
            UserAgreement inputUserAgreement = randomUserAgreement;
            UserAgreement storageUserAgreement = randomUserAgreement;
            UserAgreement expectedUserAgreement = storageUserAgreement.DeepClone();

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectUserAgreementByIdAsync(inputUserAgreement.Id))
                    .ReturnsAsync(storageUserAgreement);

            // when
            UserAgreement actualUserAgreement =
                await this.userAgreementService.RetrieveUserAgreementByIdAsync(inputUserAgreement.Id);

            // then
            actualUserAgreement.Should().BeEquivalentTo(expectedUserAgreement);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(inputUserAgreement.Id),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}