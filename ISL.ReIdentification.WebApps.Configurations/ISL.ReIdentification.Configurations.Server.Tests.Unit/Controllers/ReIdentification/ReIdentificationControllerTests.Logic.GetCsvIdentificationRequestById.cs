// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.ReIdentification
{
    public partial class ReIdentificationControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordOnGetByIdAsync()
        {
            // given
            Guid inputCsvIdentificationRequestId = Guid.NewGuid();
            CsvIdentificationRequest returnedCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            AccessRequest inputAccessRequest = randomAccessRequest;
            inputAccessRequest.CsvIdentificationRequest = returnedCsvIdentificationRequest;
            AccessRequest addedAccessRequest = inputAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = addedAccessRequest.DeepClone();
            string contentType = "text/csv";
            string fileName = "data.csv";
            var expectedObjectResult = File(expectedAccessRequest.CsvIdentificationRequest.Data, contentType, fileName);

            var expectedActionResult = expectedObjectResult;

            csvIdentificationRequestService
               .Setup(service => service.RetrieveCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId))
                   .ReturnsAsync(returnedCsvIdentificationRequest);

            identificationCoordinationServiceMock
                .Setup(service => service.ReIdentifyCsvIdentificationRequestAsync(It.IsAny<AccessRequest>()))
                    .ReturnsAsync(addedAccessRequest);

            // when
            ActionResult actualActionResult = await reIdentificationController
                .GetCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            csvIdentificationRequestService
               .Verify(service => service.RetrieveCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId),
                   Times.Once);

            identificationCoordinationServiceMock
               .Verify(service => service.ReIdentifyCsvIdentificationRequestAsync(It.IsAny<AccessRequest>()),
                   Times.Once);

            csvIdentificationRequestService.VerifyNoOtherCalls();
            identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
