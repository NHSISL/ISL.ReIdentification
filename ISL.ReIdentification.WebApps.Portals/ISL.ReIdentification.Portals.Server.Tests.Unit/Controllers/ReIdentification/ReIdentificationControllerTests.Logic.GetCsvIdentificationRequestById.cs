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

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.ReIdentification
{
    public partial class ReIdentificationControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordOnGetByIdAsync()
        {
            // given
            Guid inputCsvIdentificationRequestId = Guid.NewGuid();
            string inputReason = GetRandomString();
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            AccessRequest inputAccessRequest = randomAccessRequest;
            AccessRequest addedAccessRequest = inputAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = addedAccessRequest.DeepClone();
            string contentType = "text/csv";

            var expectedObjectResult = File(
                expectedAccessRequest.CsvIdentificationRequest.Data,
                contentType,
                expectedAccessRequest.CsvIdentificationRequest.Filepath);

            var expectedActionResult = expectedObjectResult;

            identificationCoordinationServiceMock.Setup(service =>
                service.ProcessCsvIdentificationRequestAsync(inputCsvIdentificationRequestId, inputReason))
                    .ReturnsAsync(addedAccessRequest);

            // when
            ActionResult actualActionResult = await reIdentificationController
                .GetCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId, inputReason);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            identificationCoordinationServiceMock.Verify(service =>
                service.ProcessCsvIdentificationRequestAsync(inputCsvIdentificationRequestId, inputReason),
                    Times.Once);

            identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
