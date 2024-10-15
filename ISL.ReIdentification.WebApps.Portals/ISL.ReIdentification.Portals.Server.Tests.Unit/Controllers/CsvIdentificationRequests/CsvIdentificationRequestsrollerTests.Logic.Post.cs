// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            CsvIdentificationRequest inputCsvIdentificationRequest = randomCsvIdentificationRequest;
            CsvIdentificationRequest addedCsvIdentificationRequest = inputCsvIdentificationRequest.DeepClone();
            CsvIdentificationRequest expectedCsvIdentificationRequest = addedCsvIdentificationRequest.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedCsvIdentificationRequest);

            var expectedActionResult =
                new ActionResult<CsvIdentificationRequest>(expectedObjectResult);

            csvIdentificationRequestServiceMock
                .Setup(service => service.AddCsvIdentificationRequestAsync(inputCsvIdentificationRequest))
                    .ReturnsAsync(addedCsvIdentificationRequest);

            // when
            ActionResult<CsvIdentificationRequest> actualActionResult = await csvIdentificationRequestsController
                .PostCsvIdentificationRequestAsync(randomCsvIdentificationRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            csvIdentificationRequestServiceMock
               .Verify(service => service.AddCsvIdentificationRequestAsync(inputCsvIdentificationRequest),
                   Times.Once);

            csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }
    }
}
