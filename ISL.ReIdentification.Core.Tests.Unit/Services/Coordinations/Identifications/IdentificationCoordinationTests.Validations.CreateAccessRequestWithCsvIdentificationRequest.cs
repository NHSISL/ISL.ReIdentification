// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        //[Fact]
        //public async Task
        //    ShouldThrowValidationOnCreateAccessRequestWithCsvIdentificationRequestIfIdentifierColumnNotFoundAndLogItAsync()
        //{
        //    // given
        //    Guid randomImpersonationContextId = Guid.NewGuid();
        //    Guid inputImpersonationContextId = randomImpersonationContextId;
        //    string inputContainer = randomImpersonationContextId.ToString();
        //    string inputFilepath = GetRandomString();
        //    string pseudoIdentifier = "0000000001";
        //    string randomHeaderValue = GetRandomStringWithLengthOf(10);
        //    string randomIdentifierHeaderValue = GetRandomStringWithLengthOf(10);
        //    string randomInvalidIdentifierHeaderValue = GetRandomStringWithLengthOf(10);
        //    string impersonationContextIdentifierHeaderValue = randomHeaderValue;
        //    string invalidIdentifierHeaderValue = randomIdentifierHeaderValue;
        //    string randomValue = GetRandomStringWithLengthOf(10);
        //    StringBuilder retrievedCsv = new StringBuilder();

        //    retrievedCsv.AppendLine(
        //        $"{randomHeaderValue}0,{randomHeaderValue}1,{invalidIdentifierHeaderValue}");

        //    retrievedCsv.AppendLine($"{randomValue},{randomValue},{pseudoIdentifier}");
        //    string retrievedCsvString = retrievedCsv.ToString();
        //    byte[] retrievedCsvData = Encoding.UTF8.GetBytes(retrievedCsvString);
        //    MemoryStream randomStream = new MemoryStream();
        //    MemoryStream returnedStream = new MemoryStream(retrievedCsvData);
        //    MemoryStream outputStream = randomStream;
        //    AccessRequest randomAccessRequest = CreateRandomAccessRequest();
        //    AccessRequest retrievedAccessRequest = randomAccessRequest;
        //    retrievedAccessRequest.ImpersonationContext.IdentifierColumn = impersonationContextIdentifierHeaderValue;

        //    this.identificationOrchestrationServiceMock
        //        .Setup(service => service
        //            .RetrieveDocumentByFileNameAsync(It.Is(SameStreamAs(outputStream)), inputFilepath, inputContainer))
        //                .Callback<Stream, string, string>((output, fileName, container) =>
        //                {
        //                    output.Position = 0;
        //                    returnedStream.CopyTo(output);
        //                })
        //                .Returns(ValueTask.CompletedTask);

        //    this.persistanceOrchestrationServiceMock.Setup(service =>
        //        service.RetrieveImpersonationContextByIdAsync(inputImpersonationContextId))
        //            .ReturnsAsync(retrievedAccessRequest);

        //    var service = new IdentificationCoordinationService(
        //        accessOrchestrationService: this.accessOrchestrationServiceMock.Object,
        //        persistanceOrchestrationService: this.persistanceOrchestrationServiceMock.Object,
        //        identificationOrchestrationService: this.identificationOrchestrationServiceMock.Object,
        //        csvHelperBroker: this.csvHelperBrokerMock.Object,
        //        securityBroker: this.securityBrokerMock.Object,
        //        loggingBroker: this.loggingBrokerMock.Object,
        //        dateTimeBroker: this.dateTimeBrokerMock.Object,
        //        projectStorageConfiguration: this.projectStorageConfiguration);

        //    var invalidCsvIdentificationCoordinationException =
        //        new InvalidCsvIdentificationCoordinationException(
        //        message: "Invalid csv file. Please check that the provided file has a column name " +
        //            "that matches the identifier column name given when creating the project.");

        //    var expectedIdentificationCoordinationValidationException =
        //        new IdentificationCoordinationValidationException(
        //            message: "Identification coordination validation error occurred, " +
        //                "fix the errors and try again.",
        //            innerException: invalidCsvIdentificationCoordinationException);

        //    // when
        //    ValueTask<AccessRequest> createAccessRequestTask = service
        //        .ProcessImpersonationContextRequestAsync(inputContainer, inputFilepath);

        //    IdentificationCoordinationValidationException actualIdentificationCoordinationValidationException =
        //        await Assert.ThrowsAsync<IdentificationCoordinationValidationException>(
        //            testCode: createAccessRequestTask.AsTask);

        //    // then
        //    actualIdentificationCoordinationValidationException
        //        .Should().BeEquivalentTo(expectedIdentificationCoordinationValidationException);

        //    this.persistanceOrchestrationServiceMock.Verify(service =>
        //        service.RetrieveImpersonationContextByIdAsync(inputImpersonationContextId),
        //            Times.Once);

        //    this.identificationOrchestrationServiceMock.Verify(service =>
        //        service.RetrieveDocumentByFileNameAsync(
        //            It.IsAny<Stream>(),
        //            inputFilepath,
        //            inputContainer),
        //                Times.Once);

        //    this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
        //    this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
        //}
    }
}
