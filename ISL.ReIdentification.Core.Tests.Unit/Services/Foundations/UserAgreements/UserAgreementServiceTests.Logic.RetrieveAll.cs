// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAgreements
{
    public partial class UserAgreementServiceTests
    {
        [Fact]
        public async Task ShouldReturnUserAgreements()
        {
            // given
            IQueryable<UserAgreement> randomUserAgreements = CreateRandomUserAgreements();
            IQueryable<UserAgreement> storageUserAgreements = randomUserAgreements;
            IQueryable<UserAgreement> expectedUserAgreements = storageUserAgreements;

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectAllUserAgreementsAsync())
                    .ReturnsAsync(storageUserAgreements);

            // when
            IQueryable<UserAgreement> actualUserAgreements =
                await this.userAgreementService.RetrieveAllUserAgreementsAsync();

            // then
            actualUserAgreements.Should().BeEquivalentTo(expectedUserAgreements);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectAllUserAgreementsAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}