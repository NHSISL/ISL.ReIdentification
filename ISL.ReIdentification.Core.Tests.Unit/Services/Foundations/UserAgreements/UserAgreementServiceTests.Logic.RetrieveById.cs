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

            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}