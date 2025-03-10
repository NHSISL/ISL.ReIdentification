// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAgreements
{
    public partial class UserAgreementServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedUserAgreementStorageException =
                new FailedUserAgreementStorageException(
                    message: "Failed userAgreement storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedUserAgreementDependencyException =
                new UserAgreementDependencyException(
                    message: "UserAgreement dependency error occurred, contact support.",
                    innerException: failedUserAgreementStorageException);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<UserAgreement> retrieveUserAgreementByIdTask =
                this.userAgreementService.RetrieveUserAgreementByIdAsync(someId);

            UserAgreementDependencyException actualUserAgreementDependencyException =
                await Assert.ThrowsAsync<UserAgreementDependencyException>(
                    retrieveUserAgreementByIdTask.AsTask);

            // then
            actualUserAgreementDependencyException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyException);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedUserAgreementDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedUserAgreementServiceException =
                new FailedUserAgreementServiceException(
                    message: "Failed userAgreement service occurred, please contact support",
                    innerException: serviceException);

            var expectedUserAgreementServiceException =
                new UserAgreementServiceException(
                    message: "UserAgreement service error occurred, contact support.",
                    innerException: failedUserAgreementServiceException);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<UserAgreement> retrieveUserAgreementByIdTask =
                this.userAgreementService.RetrieveUserAgreementByIdAsync(someId);

            UserAgreementServiceException actualUserAgreementServiceException =
                await Assert.ThrowsAsync<UserAgreementServiceException>(
                    retrieveUserAgreementByIdTask.AsTask);

            // then
            actualUserAgreementServiceException.Should()
                .BeEquivalentTo(expectedUserAgreementServiceException);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedUserAgreementServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}