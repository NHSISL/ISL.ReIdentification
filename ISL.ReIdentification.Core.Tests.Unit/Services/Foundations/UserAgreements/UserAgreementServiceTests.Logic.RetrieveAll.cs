using System.Linq;
using FluentAssertions;
using Moq;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using Xunit;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAgreements
{
    public partial class UserAgreementServiceTests
    {
        [Fact]
        public void ShouldReturnUserAgreements()
        {
            // given
            IQueryable<UserAgreement> randomUserAgreements = CreateRandomUserAgreements();
            IQueryable<UserAgreement> storageUserAgreements = randomUserAgreements;
            IQueryable<UserAgreement> expectedUserAgreements = storageUserAgreements;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUserAgreements())
                    .Returns(storageUserAgreements);

            // when
            IQueryable<UserAgreement> actualUserAgreements =
                this.userAgreementService.RetrieveAllUserAgreements();

            // then
            actualUserAgreements.Should().BeEquivalentTo(expectedUserAgreements);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUserAgreements(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}