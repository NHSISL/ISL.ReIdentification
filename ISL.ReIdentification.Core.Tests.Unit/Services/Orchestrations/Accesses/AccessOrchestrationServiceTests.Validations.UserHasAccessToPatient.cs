// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(GetTestInvalidParameters))]
        public async Task ShouldThrowValidationExceptionOnUserHasAccessToPatientIfInvalidAndLogItAsync(
            string invalidIdentifier,
            List<string> invalidOrganisations)
        {
            // Given
            var invalidArgumentAccessOrchestrationException =
                new InvalidArgumentAccessOrchestrationException(
                    message: "Invalid argument access orchestration exception, " +
                        "please correct the errors and try again.");

            invalidArgumentAccessOrchestrationException.AddData(
                key: "identifier",
                values: "Text is invalid");

            invalidArgumentAccessOrchestrationException.AddData(
                key: "orgs",
                values: "List is invalid");

            var expectedAccessValidationOrchestrationException = invalidArgumentAccessOrchestrationException;

            // When
            ValueTask<bool> getUserHasAccessToPatientTask = this.accessOrchestrationService
                .UserHasAccessToPatientAsync(invalidIdentifier, invalidOrganisations);

            InvalidArgumentAccessOrchestrationException actualAccessValidationOrchestrationException =
                await Assert.ThrowsAsync<InvalidArgumentAccessOrchestrationException>(
                    testCode: getUserHasAccessToPatientTask.AsTask);

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
