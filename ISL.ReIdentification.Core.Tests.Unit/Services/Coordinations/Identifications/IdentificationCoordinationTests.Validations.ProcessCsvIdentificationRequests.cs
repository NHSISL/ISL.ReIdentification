// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnProcessIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidCsvIdentificationRequestId = Guid.Empty;

            var invalidCsvIdentificationRequestException =
                new InvalidCsvIdentificationRequestException(
                    message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.Id),
                values: "Id is invalid");

            var expectedCsvIdentificationRequestValidationException =
                new IdentificationCoordinationValidationException(
                    message: "Identification coordination validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            // when
            ValueTask<AccessRequest> processCsvIdentificationRequestTask =
                this.identificationCoordinationService
                    .ProcessCsvIdentificationRequestAsync(invalidCsvIdentificationRequestId);

            IdentificationCoordinationValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<IdentificationCoordinationValidationException>(
                    testCode: processCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedCsvIdentificationRequestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
