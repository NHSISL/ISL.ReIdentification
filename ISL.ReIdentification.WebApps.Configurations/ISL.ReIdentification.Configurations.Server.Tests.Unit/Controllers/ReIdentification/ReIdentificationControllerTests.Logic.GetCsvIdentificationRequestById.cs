// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
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
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            AccessRequest inputAccessRequest = randomAccessRequest;
            AccessRequest addedAccessRequest = inputAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = addedAccessRequest.DeepClone();
            string contentType = "text/csv";
            string fileName = "data.csv";
            var expectedObjectResult = File(expectedAccessRequest.CsvIdentificationRequest.Data, contentType, fileName);

            var expectedActionResult = expectedObjectResult;

            identificationCoordinationServiceMock
                .Setup(service => service.ProcessCsvIdentificationRequestAsync(inputCsvIdentificationRequestId))
                    .ReturnsAsync(addedAccessRequest);

            // when
            ActionResult actualActionResult = await reIdentificationController
                .GetCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            identificationCoordinationServiceMock
               .Verify(service => service.ProcessCsvIdentificationRequestAsync(inputCsvIdentificationRequestId),
                   Times.Once);

            identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
