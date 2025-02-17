// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAgreements
{
    public partial class UserAgreementServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            UserAgreement someUserAgreement = CreateRandomUserAgreement();
            SqlException sqlException = GetSqlException();

            var failedUserAgreementStorageException =
                new FailedUserAgreementStorageException(
                    message: "Failed userAgreement storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedUserAgreementDependencyException =
                new UserAgreementDependencyException(
                    message: "UserAgreement dependency error occurred, contact support.",
                    innerException: failedUserAgreementStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<UserAgreement> addUserAgreementTask =
                this.userAgreementService.AddUserAgreementAsync(someUserAgreement);

            UserAgreementDependencyException actualUserAgreementDependencyException =
                await Assert.ThrowsAsync<UserAgreementDependencyException>(
                    addUserAgreementTask.AsTask);

            // then
            actualUserAgreementDependencyException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

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
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfUserAgreementAlreadyExsitsAndLogItAsync()
        {
            // given
            UserAgreement randomUserAgreement = CreateRandomUserAgreement();
            UserAgreement alreadyExistsUserAgreement = randomUserAgreement;
            string randomMessage = GetRandomString();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsUserAgreementException =
                new AlreadyExistsUserAgreementException(
                    message: "UserAgreement with the same Id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedUserAgreementDependencyValidationException =
                new UserAgreementDependencyValidationException(
                    message: "UserAgreement dependency validation occurred, please try again.",
                    innerException: alreadyExistsUserAgreementException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<UserAgreement> addUserAgreementTask =
                this.userAgreementService.AddUserAgreementAsync(alreadyExistsUserAgreement);

            // then
            UserAgreementDependencyValidationException actualUserAgreementDependencyValidationException =
                await Assert.ThrowsAsync<UserAgreementDependencyValidationException>(
                    addUserAgreementTask.AsTask);

            actualUserAgreementDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAgreementDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            UserAgreement someUserAgreement = CreateRandomUserAgreement();
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidUserAgreementReferenceException =
                new InvalidUserAgreementReferenceException(
                    message: "Invalid userAgreement reference error occurred.",
                    innerException: foreignKeyConstraintConflictException);

            var expectedUserAgreementValidationException =
                new UserAgreementDependencyValidationException(
                    message: "UserAgreement dependency validation occurred, please try again.",
                    innerException: invalidUserAgreementReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<UserAgreement> addUserAgreementTask =
                this.userAgreementService.AddUserAgreementAsync(someUserAgreement);

            // then
            UserAgreementDependencyValidationException actualUserAgreementDependencyValidationException =
                await Assert.ThrowsAsync<UserAgreementDependencyValidationException>(
                    addUserAgreementTask.AsTask);

            actualUserAgreementDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAgreementValidationException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(someUserAgreement),
                    Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            UserAgreement someUserAgreement = CreateRandomUserAgreement();

            var databaseUpdateException =
                new DbUpdateException();

            var failedUserAgreementStorageException =
                new FailedUserAgreementStorageException(
                    message: "Failed userAgreement storage error occurred, contact support.",
                    innerException: databaseUpdateException);

            var expectedUserAgreementDependencyException =
                new UserAgreementDependencyException(
                    message: "UserAgreement dependency error occurred, contact support.",
                    innerException: failedUserAgreementStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<UserAgreement> addUserAgreementTask =
                this.userAgreementService.AddUserAgreementAsync(someUserAgreement);

            UserAgreementDependencyException actualUserAgreementDependencyException =
                await Assert.ThrowsAsync<UserAgreementDependencyException>(
                    addUserAgreementTask.AsTask);

            // then
            actualUserAgreementDependencyException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAgreementDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            UserAgreement someUserAgreement = CreateRandomUserAgreement();
            var serviceException = new Exception();

            var failedUserAgreementServiceException =
                new FailedUserAgreementServiceException(
                    message: "Failed userAgreement service occurred, please contact support",
                    innerException: serviceException);

            var expectedUserAgreementServiceException =
                new UserAgreementServiceException(
                    message: "UserAgreement service error occurred, contact support.",
                    innerException: failedUserAgreementServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<UserAgreement> addUserAgreementTask =
                this.userAgreementService.AddUserAgreementAsync(someUserAgreement);

            UserAgreementServiceException actualUserAgreementServiceException =
                await Assert.ThrowsAsync<UserAgreementServiceException>(
                    addUserAgreementTask.AsTask);

            // then
            actualUserAgreementServiceException.Should()
                .BeEquivalentTo(expectedUserAgreementServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

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