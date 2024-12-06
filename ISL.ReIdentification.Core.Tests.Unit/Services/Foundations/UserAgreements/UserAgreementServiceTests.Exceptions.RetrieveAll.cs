using System;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;
using Xunit;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAgreements
{
    public partial class UserAgreementServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedUserAgreementStorageException =
                new FailedUserAgreementStorageException(
                    message: "Failed userAgreement storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedUserAgreementDependencyException =
                new UserAgreementDependencyException(
                    message: "UserAgreement dependency error occurred, contact support.",
                    innerException: failedUserAgreementStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUserAgreements())
                    .Throws(sqlException);

            // when
            Action retrieveAllUserAgreementsAction = () =>
                this.userAgreementService.RetrieveAllUserAgreements();

            UserAgreementDependencyException actualUserAgreementDependencyException =
                Assert.Throws<UserAgreementDependencyException>(retrieveAllUserAgreementsAction);

            // then
            actualUserAgreementDependencyException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUserAgreements(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserAgreementDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedUserAgreementServiceException =
                new FailedUserAgreementServiceException(
                    message: "Failed userAgreement service occurred, please contact support", 
                    innerException: serviceException);

            var expectedUserAgreementServiceException =
                new UserAgreementServiceException(
                    message: "UserAgreement service error occurred, contact support.",
                    innerException: failedUserAgreementServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUserAgreements())
                    .Throws(serviceException);

            // when
            Action retrieveAllUserAgreementsAction = () =>
                this.userAgreementService.RetrieveAllUserAgreements();

            UserAgreementServiceException actualUserAgreementServiceException =
                Assert.Throws<UserAgreementServiceException>(retrieveAllUserAgreementsAction);

            // then
            actualUserAgreementServiceException.Should()
                .BeEquivalentTo(expectedUserAgreementServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUserAgreements(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserAgreementServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}