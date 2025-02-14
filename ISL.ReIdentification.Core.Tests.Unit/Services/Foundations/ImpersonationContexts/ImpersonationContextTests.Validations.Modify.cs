// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Services.Foundations.Lookups;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.ImpersonationContexts
{
    public partial class ImpersonationContextsTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfImpersonationContextIsNullAndLogItAsync()
        {
            //given
            ImpersonationContext nullImpersonationContext = null;

            var nullImpersonationContextException =
                new NullImpersonationContextException(message: "Impersonation context is null.");

            var expectedImpersonationContextValidationException =
                new ImpersonationContextValidationException(
                    message: "Impersonation context validation error occurred, please fix errors and try again.",
                    innerException: nullImpersonationContextException);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                this.impersonationContextService.ModifyImpersonationContextAsync(nullImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedImpersonationContextValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateImpersonationContextAsync(It.IsAny<ImpersonationContext>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfImpersonationContextIsInvalidAndLogItAsync(
            string invalidText)
        {
            //given
            EntraUser randomEntraUser = CreateRandomEntraUser();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);

            var invalidImpersonationContext = new ImpersonationContext
            {
                RequesterEntraUserId = invalidText,
                RequesterEmail = invalidText,
                ResponsiblePersonEntraUserId = invalidText,
                ResponsiblePersonEmail = invalidText,
                IdentifierColumn = invalidText,
                ProjectName = invalidText,
                CreatedBy = invalidText,
                UpdatedBy = invalidText,
            };

            var impersonationContextServiceMock = new Mock<ImpersonationContextService>(
                this.reIdentificationStorageBroker.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            impersonationContextServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(invalidImpersonationContext))
                    .ReturnsAsync(invalidImpersonationContext);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidImpersonationContextException = new InvalidImpersonationContextException(
                message: "Invalid impersonation context. Please correct the errors and try again.");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.Id),
                values: "Id is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.RequesterEntraUserId),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.RequesterEmail),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.ResponsiblePersonEntraUserId),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.ResponsiblePersonEmail),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.IdentifierColumn),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.ProjectName),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.CreatedBy),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.CreatedDate),
                values: "Date is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.UpdatedBy),
                values:
                    [
                        "Text is invalid",
                        $"Expected value to be '{randomEntraUser.EntraUserId}' but found '{invalidText}'."
                    ]);

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.UpdatedDate),
                values:
                new[] {
                    "Date is invalid",
                    $"Date is the same as {nameof(ImpersonationContext.CreatedDate)}",
                    $"Date is not recent. Expected a value between {startDate} and {endDate} but found " +
                        $"{invalidImpersonationContext.UpdatedDate}"
                });

            var expectedImpersonationContextValidationException =
                new ImpersonationContextValidationException(
                    message: "Impersonation context validation error occurred, please fix errors and try again.",
                    innerException: invalidImpersonationContextException);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                impersonationContextServiceMock.Object.ModifyImpersonationContextAsync(invalidImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedImpersonationContextValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateImpersonationContextAsync(It.IsAny<ImpersonationContext>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnModifyIfImpersonationContextHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser(entraUserId: GetRandomStringWithLengthOf(256));
            
            ImpersonationContext invalidImpersonationContext = CreateRandomImpersonationContext(
                randomDateTimeOffset, impersonationContextId: randomEntraUser.EntraUserId);
            
            var inputCreatedByUpdatedByString = randomEntraUser.EntraUserId;
            invalidImpersonationContext.RequesterEmail = GetRandomStringWithLengthOf(321);
            invalidImpersonationContext.ResponsiblePersonEmail = GetRandomStringWithLengthOf(321);
            invalidImpersonationContext.IdentifierColumn = GetRandomStringWithLengthOf(11);
            invalidImpersonationContext.ProjectName = GetRandomStringWithLengthOf(256);
            invalidImpersonationContext.CreatedBy = inputCreatedByUpdatedByString;
            invalidImpersonationContext.UpdatedBy = inputCreatedByUpdatedByString;

            var impersonationContextServiceMock = new Mock<ImpersonationContextService>(
                this.reIdentificationStorageBroker.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            impersonationContextServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(invalidImpersonationContext))
                    .ReturnsAsync(invalidImpersonationContext);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidImpersonationContextException =
                new InvalidImpersonationContextException(
                    message: "Invalid impersonation context. Please correct the errors and try again.");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.RequesterEmail),
                values: $"Text exceed max length of " +
                    $"{invalidImpersonationContext.RequesterEmail.Length - 1} characters");


            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.ResponsiblePersonEmail),
                values: $"Text exceed max length of " +
                    $"{invalidImpersonationContext.ResponsiblePersonEmail.Length - 1} characters");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.IdentifierColumn),
                values: $"Text exceed max length of " +
                    $"{invalidImpersonationContext.IdentifierColumn.Length - 1} characters");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.ProjectName),
                values: $"Text exceed max length of " +
                    $"{invalidImpersonationContext.ProjectName.Length - 1} characters");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.CreatedBy),
                values: $"Text exceed max length of " +
                    $"{invalidImpersonationContext.CreatedBy.Length - 1} characters");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.UpdatedBy),
                values: $"Text exceed max length of " +
                    $"{invalidImpersonationContext.UpdatedBy.Length - 1} characters");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.UpdatedDate),
                values: $"Date is the same as {nameof(ImpersonationContext.CreatedDate)}");

            var expectedImpersonationContextValidationException =
                new ImpersonationContextValidationException(
                    message: "Impersonation context validation error occurred, please fix errors and try again.",
                    innerException: invalidImpersonationContextException);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                impersonationContextServiceMock.Object.ModifyImpersonationContextAsync(invalidImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should()
                .BeEquivalentTo(expectedImpersonationContextValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertImpersonationContextAsync(It.IsAny<ImpersonationContext>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext(
                randomDateTimeOffset,
                impersonationContextId: randomEntraUser.EntraUserId);

            ImpersonationContext invalidImpersonationContext = randomImpersonationContext;

            var impersonationContextServiceMock = new Mock<ImpersonationContextService>(
                this.reIdentificationStorageBroker.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            impersonationContextServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(invalidImpersonationContext))
                    .ReturnsAsync(invalidImpersonationContext);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidImpersonationContextException = new InvalidImpersonationContextException(
                message: "Invalid impersonation context. Please correct the errors and try again.");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.UpdatedDate),
                values: $"Date is the same as {nameof(ImpersonationContext.CreatedDate)}");

            var expectedImpersonationContextValidationException = new ImpersonationContextValidationException(
                message: "Impersonation context validation error occurred, please fix errors and try again.",
                innerException: invalidImpersonationContextException);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                impersonationContextServiceMock.Object.ModifyImpersonationContextAsync(invalidImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedImpersonationContextValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-91)]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);
            EntraUser randomEntraUser = CreateRandomEntraUser();

            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext(
                randomDateTimeOffset, 
                impersonationContextId: randomEntraUser.EntraUserId);

            ImpersonationContext invalidImpersonationContext = randomImpersonationContext;
            randomImpersonationContext.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var impersonationContextServiceMock = new Mock<ImpersonationContextService>(
                this.reIdentificationStorageBroker.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            impersonationContextServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(invalidImpersonationContext))
                    .ReturnsAsync(invalidImpersonationContext);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidImpersonationContextException = new InvalidImpersonationContextException(
                message: "Invalid impersonation context. Please correct the errors and try again.");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.UpdatedDate),
                values:
                [
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but " +
                    $"found {randomImpersonationContext.UpdatedDate}"
                ]);

            var expectedImpersonationContextValidationException = new ImpersonationContextValidationException(
                message: "Impersonation context validation error occurred, please fix errors and try again.",
                innerException: invalidImpersonationContextException);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                impersonationContextServiceMock.Object.ModifyImpersonationContextAsync(randomImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedImpersonationContextValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageImpersonationContextDoesNotExistAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext(
                randomDateTimeOffset, 
                impersonationContextId: randomEntraUser.EntraUserId);
            
            ImpersonationContext nonExistingImpersonationContext = randomImpersonationContext;
            ImpersonationContext nullImpersonationContext = null;

            var impersonationContextServiceMock = new Mock<ImpersonationContextService>(
                this.reIdentificationStorageBroker.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            impersonationContextServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(nonExistingImpersonationContext))
                    .ReturnsAsync(nonExistingImpersonationContext);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var notFoundImpersonationContextException =
                new NotFoundImpersonationContextException(
                    message: $"Impersonation context not found with id: {nonExistingImpersonationContext.Id}");

            var expectedImpersonationContextValidationException =
                new ImpersonationContextValidationException(
                    message: "Impersonation context validation error occurred, please fix errors and try again.",
                    innerException: notFoundImpersonationContextException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectImpersonationContextByIdAsync(nonExistingImpersonationContext.Id))
                    .ReturnsAsync(nullImpersonationContext);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                impersonationContextServiceMock.Object.ModifyImpersonationContextAsync(
                    nonExistingImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(nonExistingImpersonationContext.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextValidationException))),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedAuditInfoHasChangedAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            ImpersonationContext randomImpersonationContext =
                CreateRandomModifyImpersonationContext(randomDateTimeOffset, impersonationContextId: randomEntraUser.EntraUserId);

            ImpersonationContext invalidImpersonationContext = randomImpersonationContext;
            ImpersonationContext storedImpersonationContext = randomImpersonationContext.DeepClone();
            storedImpersonationContext.CreatedBy = GetRandomString();
            storedImpersonationContext.CreatedDate = storedImpersonationContext.CreatedDate.AddMinutes(GetRandomNegativeNumber());
            Guid impersonationContextId = invalidImpersonationContext.Id;

            var impersonationContextServiceMock = new Mock<ImpersonationContextService>(
                this.reIdentificationStorageBroker.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            impersonationContextServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(invalidImpersonationContext))
                    .ReturnsAsync(invalidImpersonationContext);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectImpersonationContextByIdAsync(impersonationContextId))
                    .ReturnsAsync(storedImpersonationContext);

            var invalidImpersonationContextException = new InvalidImpersonationContextException(
                message: "Invalid impersonation context. Please correct the errors and try again.");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.CreatedBy),
                values: $"Text is not the same as {nameof(ImpersonationContext.CreatedBy)}");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.CreatedDate),
                values: $"Date is not the same as {nameof(ImpersonationContext.CreatedDate)}");

            var expectedImpersonationContextValidationException =
                new ImpersonationContextValidationException(
                    message: "Impersonation context validation error occurred, please fix errors and try again.",
                    innerException: invalidImpersonationContextException);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                impersonationContextServiceMock.Object.ModifyImpersonationContextAsync(invalidImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(invalidImpersonationContext.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedImpersonationContextValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            ImpersonationContext randomImpersonationContext =
                CreateRandomModifyImpersonationContext(
                    randomDateTimeOffset, impersonationContextId: randomEntraUser.EntraUserId);

            ImpersonationContext invalidImpersonationContext = randomImpersonationContext;
            ImpersonationContext storageImpersonationContext = randomImpersonationContext.DeepClone();

            var impersonationContextServiceMock = new Mock<ImpersonationContextService>(
                this.reIdentificationStorageBroker.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            impersonationContextServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(invalidImpersonationContext))
                    .ReturnsAsync(invalidImpersonationContext);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidImpersonationContextException = new InvalidImpersonationContextException(
                message: "Invalid impersonation context. Please correct the errors and try again.");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.UpdatedDate),
                values: $"Date is the same as {nameof(ImpersonationContext.UpdatedDate)}");

            var expectedImpersonationContextValidationException =
                new ImpersonationContextValidationException(
                    message: "Impersonation context validation error occurred, please fix errors and try again.",
                    innerException: invalidImpersonationContextException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectImpersonationContextByIdAsync(invalidImpersonationContext.Id))
                .ReturnsAsync(storageImpersonationContext);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                impersonationContextServiceMock.Object.ModifyImpersonationContextAsync(invalidImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
               await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                   testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(invalidImpersonationContext.Id),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
