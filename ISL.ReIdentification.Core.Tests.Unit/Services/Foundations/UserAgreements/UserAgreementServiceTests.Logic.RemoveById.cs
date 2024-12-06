using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using Xunit;

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

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserAgreementByIdAsync(inputUserAgreementId))
                    .ReturnsAsync(storageUserAgreement);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteUserAgreementAsync(expectedInputUserAgreement))
                    .ReturnsAsync(deletedUserAgreement);

            // when
            UserAgreement actualUserAgreement = await this.userAgreementService
                .RemoveUserAgreementByIdAsync(inputUserAgreementId);

            // then
            actualUserAgreement.Should().BeEquivalentTo(expectedUserAgreement);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(inputUserAgreementId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteUserAgreementAsync(expectedInputUserAgreement),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}