// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;
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
            var invalidImpersonationContext = new ImpersonationContext
            {
                RequesterEmail = invalidText,
                RecipientEmail = invalidText,
                IdentifierColumn = invalidText,
                PipelineName = invalidText,
                ManagedIdentityName = invalidText,
            };

            var invalidImpersonationContextException = new InvalidImpersonationContextException(
                message: "Invalid impersonation context. Please correct the errors and try again.");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.Id),
                values: "Id is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.RequesterEntraUserId),
                values: "Id is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.RequesterEmail),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.RecipientEntraUserId),
                values: "Id is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.RecipientEmail),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.IdentifierColumn),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.PipelineName),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.ManagedIdentityName),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.CreatedBy),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.UpdatedBy),
                values: "Text is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.CreatedDate),
                values: "Date is invalid");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.UpdatedDate),
                new[]
                    {
                        "Date is invalid",
                        $"Date is the same as {nameof(ImpersonationContext.CreatedDate)}"
                    });

            var expectedImpersonationContextValidationException =
                new ImpersonationContextValidationException(
                    message: "Impersonation context validation error occurred, please fix errors and try again.",
                    innerException: invalidImpersonationContextException);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                this.impersonationContextService.ModifyImpersonationContextAsync(invalidImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedImpersonationContextValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateImpersonationContextAsync(It.IsAny<ImpersonationContext>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnModifyIfImpersonationContextHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            var invalidImpersonationContext = CreateRandomImpersonationContext(dateTimeOffset: randomDateTimeOffset);
            var username = GetRandomStringWithLengthOf(256);
            invalidImpersonationContext.RequesterEmail = GetRandomStringWithLengthOf(321);
            invalidImpersonationContext.RecipientEmail = GetRandomStringWithLengthOf(321);
            invalidImpersonationContext.IdentifierColumn = GetRandomStringWithLengthOf(11);
            invalidImpersonationContext.PipelineName = GetRandomStringWithLengthOf(256);
            invalidImpersonationContext.CreatedBy = username;
            invalidImpersonationContext.UpdatedBy = username;

            var invalidImpersonationContextException =
                new InvalidImpersonationContextException(
                    message: "Invalid impersonation context. Please correct the errors and try again.");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.RequesterEmail),
                values: $"Text exceed max length of " +
                    $"{invalidImpersonationContext.RequesterEmail.Length - 1} characters");


            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.RecipientEmail),
                values: $"Text exceed max length of " +
                    $"{invalidImpersonationContext.RecipientEmail.Length - 1} characters");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.IdentifierColumn),
                values: $"Text exceed max length of " +
                    $"{invalidImpersonationContext.IdentifierColumn.Length - 1} characters");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.PipelineName),
                values: $"Text exceed max length of " +
                    $"{invalidImpersonationContext.PipelineName.Length - 1} characters");

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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                this.impersonationContextService.ModifyImpersonationContextAsync(invalidImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should()
                .BeEquivalentTo(expectedImpersonationContextValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertImpersonationContextAsync(It.IsAny<ImpersonationContext>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext(randomDateTimeOffset);
            ImpersonationContext invalidImpersonationContext = randomImpersonationContext;

            var invalidImpersonationContextException = new InvalidImpersonationContextException(
                message: "Invalid impersonation context. Please correct the errors and try again.");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.UpdatedDate),
                values: $"Date is the same as {nameof(ImpersonationContext.CreatedDate)}");

            var expectedImpersonationContextValidationException = new ImpersonationContextValidationException(
                message: "Impersonation context validation error occurred, please fix errors and try again.",
                innerException: invalidImpersonationContextException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                this.impersonationContextService.ModifyImpersonationContextAsync(invalidImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedImpersonationContextValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext(randomDateTimeOffset);
            randomImpersonationContext.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var invalidImpersonationContextException = new InvalidImpersonationContextException(
                message: "Invalid impersonation context. Please correct the errors and try again.");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.UpdatedDate),
                values:
                [
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but " + 
                    "found {randomImpersonationContext.UpdatedDate}"
                ]);

            var expectedImpersonationContextValidationException = new ImpersonationContextValidationException(
                message: "Impersonation context validation error occurred, please fix errors and try again.",
                innerException: invalidImpersonationContextException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                this.impersonationContextService.ModifyImpersonationContextAsync(randomImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedImpersonationContextValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageImpersonationContextDoesNotExistAndLogItAsync()
        {
            //given
            int randomNegative = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext(randomDateTimeOffset);
            ImpersonationContext nonExistingImpersonationContext = randomImpersonationContext;
            nonExistingImpersonationContext.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegative);
            ImpersonationContext nullImpersonationContext = null;

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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                this.impersonationContextService.ModifyImpersonationContextAsync(nonExistingImpersonationContext);

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

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextValidationException))),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedAuditInfoHasChangedAndLogItAsync()
        {
            //given
            int randomMinutes = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            
            ImpersonationContext randomImpersonationContext = 
                CreateRandomModifyImpersonationContext(randomDateTimeOffset);
            
            ImpersonationContext invalidImpersonationContext = randomImpersonationContext;
            ImpersonationContext storedImpersonationContext = randomImpersonationContext.DeepClone();
            storedImpersonationContext.CreatedBy = GetRandomString();
            storedImpersonationContext.CreatedDate = storedImpersonationContext.CreatedDate.AddMinutes(randomMinutes);
            storedImpersonationContext.UpdatedDate = storedImpersonationContext.UpdatedDate.AddMinutes(randomMinutes);
            Guid ImpersonationContextId = invalidImpersonationContext.Id;

            var invalidImpersonationContextException = new InvalidImpersonationContextException(
                message: "Invalid impersonation context. Please correct the errors and try again.");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.CreatedBy),
                values: $"Text is not the same as {nameof(ImpersonationContext.CreatedBy)}");

            invalidImpersonationContextException.AddData(
                key: nameof(ImpersonationContext.CreatedDate),
                values: $"Date is not the same as {nameof(ImpersonationContext.CreatedDate)}");

            var expectedImpersonationContextValidationException = new ImpersonationContextValidationException(
                message: "Impersonation context validation error occurred, please fix errors and try again.",
                innerException: invalidImpersonationContextException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectImpersonationContextByIdAsync(ImpersonationContextId))
                    .ReturnsAsync(storedImpersonationContext);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                this.impersonationContextService.ModifyImpersonationContextAsync(invalidImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(invalidImpersonationContext.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedImpersonationContextValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            
            ImpersonationContext randomImpersonationContext = 
                CreateRandomModifyImpersonationContext(randomDateTimeOffset);
            
            ImpersonationContext invalidImpersonationContext = randomImpersonationContext;
            ImpersonationContext storageImpersonationContext = randomImpersonationContext.DeepClone();
            invalidImpersonationContext.UpdatedDate = storageImpersonationContext.UpdatedDate;

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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                this.impersonationContextService.ModifyImpersonationContextAsync(invalidImpersonationContext);

            ImpersonationContextValidationException actualImpersonationContextValidationException =
               await Assert.ThrowsAsync<ImpersonationContextValidationException>(
                   testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(invalidImpersonationContext.Id),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
