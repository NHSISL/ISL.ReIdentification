﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAccesses
{
    public partial class UserAccessesTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllOrganisationsUserHasAccessToAsync()
        {
            // given
            Guid randomUserAccessId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedUserAccessStorageException =
                new FailedStorageUserAccessException(
                    message: "Failed user access storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedUserAccessDependencyException =
                new UserAccessDependencyException(
                    message: "UserAccess dependency error occurred, contact support.",
                    innerException: failedUserAccessStorageException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllUserAccessesAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<List<string>> retrieveAllOrganisationUserHasAccessToTask =
                this.userAccessService.RetrieveAllOrganisationsUserHasAccessTo(randomUserAccessId);

            UserAccessDependencyException actualUserAccessDependencyException =
                await Assert.ThrowsAsync<UserAccessDependencyException>(
                    testCode: retrieveAllOrganisationUserHasAccessToTask.AsTask);

            // then
            actualUserAccessDependencyException.Should().BeEquivalentTo(
                expectedUserAccessDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllUserAccessesAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedUserAccessDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllOrganisationsUserHasAccessToAsync()
        {
            // given
            Guid randomUserAccessId = Guid.NewGuid();
            Exception serviceError = new Exception();

            var failedServiceUserAccessException = new FailedServiceUserAccessException(
                message: "Failed service user access error occurred, contact support.",
                innerException: serviceError);

            var expectedUserAccessServiceException = new UserAccessServiceException(
                message: "Service error occurred, contact support.",
                innerException: failedServiceUserAccessException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllUserAccessesAsync())
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<List<string>> retrieveAllOrganisationUserHasAccessToTask =
                this.userAccessService.RetrieveAllOrganisationsUserHasAccessTo(randomUserAccessId);

            UserAccessServiceException actualUserAccessServiceExcpetion =
                await Assert.ThrowsAsync<UserAccessServiceException>(
                    testCode: retrieveAllOrganisationUserHasAccessToTask.AsTask);

            // then
            actualUserAccessServiceExcpetion.Should().BeEquivalentTo(expectedUserAccessServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllUserAccessesAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAccessServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
