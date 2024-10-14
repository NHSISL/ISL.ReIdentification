// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnGetOrganisationsForUserIfInvalidAndLogItAsync()
        {
            // Given
            Guid invalidUserEmail = Guid.Empty;

            var invalidArgumentAccessOrchestrationException =
                new InvalidArgumentAccessOrchestrationException(
                    message: "Invalid argument access orchestration exception, " +
                        "please correct the errors and try again.");

            invalidArgumentAccessOrchestrationException.AddData(
                key: "entraUserId",
                values: "Id is invalid");

            var expectedAccessValidationOrchestrationException = invalidArgumentAccessOrchestrationException;

            // When
            ValueTask<List<string>> getOrganisationsForUserTask = this.accessOrchestrationService
                .GetOrganisationsForUserAsync(invalidUserEmail);

            InvalidArgumentAccessOrchestrationException actualAccessValidationOrchestrationException =
                await Assert.ThrowsAsync<InvalidArgumentAccessOrchestrationException>(
                    testCode: getOrganisationsForUserTask.AsTask);

            // Then
            actualAccessValidationOrchestrationException.Should()
                .BeEquivalentTo(expectedAccessValidationOrchestrationException);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
