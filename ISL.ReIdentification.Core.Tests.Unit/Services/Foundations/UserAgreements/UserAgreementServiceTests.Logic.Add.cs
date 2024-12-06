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
        public async Task ShouldAddUserAgreementAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            UserAgreement randomUserAgreement = CreateRandomUserAgreement(randomDateTimeOffset);
            UserAgreement inputUserAgreement = randomUserAgreement;
            UserAgreement storageUserAgreement = inputUserAgreement;
            UserAgreement expectedUserAgreement = storageUserAgreement.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertUserAgreementAsync(inputUserAgreement))
                    .ReturnsAsync(storageUserAgreement);

            // when
            UserAgreement actualUserAgreement = await this.userAgreementService
                .AddUserAgreementAsync(inputUserAgreement);

            // then
            actualUserAgreement.Should().BeEquivalentTo(expectedUserAgreement);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(inputUserAgreement),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}