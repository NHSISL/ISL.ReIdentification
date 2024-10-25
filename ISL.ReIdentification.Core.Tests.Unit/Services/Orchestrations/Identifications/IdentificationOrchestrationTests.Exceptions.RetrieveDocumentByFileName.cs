// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        [Theory]
        [MemberData(nameof(DocumentDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRetrieveDocumentByFileNameAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            string someFilename = GetRandomString();
            string someContainer = GetRandomString();
            MemoryStream someStream = new MemoryStream();

            this.documentServiceMock.Setup(service =>
                service.RetrieveDocumentByFileNameAsync(someStream, someFilename, someContainer))
                    .ThrowsAsync(dependencyValidationException);

            var expectedIdentificationOrchestrationDependencyValidationException =
                new IdentificationOrchestrationDependencyValidationException(
                    message: "Identification orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask retrieveDocumentByFileNameTask =
                this.identificationOrchestrationService
                    .RetrieveDocumentByFileNameAsync(someStream, someFilename, someContainer);

            IdentificationOrchestrationDependencyValidationException
                actualIdentificationOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<IdentificationOrchestrationDependencyValidationException>(
                        testCode: retrieveDocumentByFileNameTask.AsTask);

            // then
            actualIdentificationOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedIdentificationOrchestrationDependencyValidationException);

            this.documentServiceMock.Verify(service =>
                service.RetrieveDocumentByFileNameAsync(someStream, someFilename, someContainer),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationOrchestrationDependencyValidationException))),
                       Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
