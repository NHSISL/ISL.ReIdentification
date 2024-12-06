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
        public async Task ShouldModifyUserAgreementAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            UserAgreement randomUserAgreement = CreateRandomModifyUserAgreement(randomDateTimeOffset);
            UserAgreement inputUserAgreement = randomUserAgreement;
            UserAgreement storageUserAgreement = inputUserAgreement.DeepClone();
            storageUserAgreement.UpdatedDate = randomUserAgreement.CreatedDate;
            UserAgreement updatedUserAgreement = inputUserAgreement;
            UserAgreement expectedUserAgreement = updatedUserAgreement.DeepClone();
            Guid userAgreementId = inputUserAgreement.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateUserAgreementAsync(inputUserAgreement))
                    .ReturnsAsync(updatedUserAgreement);

            // when
            UserAgreement actualUserAgreement =
                await this.userAgreementService.ModifyUserAgreementAsync(inputUserAgreement);

            // then
            actualUserAgreement.Should().BeEquivalentTo(expectedUserAgreement);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAgreementAsync(inputUserAgreement),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}