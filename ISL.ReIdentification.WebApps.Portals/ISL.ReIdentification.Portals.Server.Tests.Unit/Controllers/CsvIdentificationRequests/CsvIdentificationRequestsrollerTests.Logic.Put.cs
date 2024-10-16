// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            CsvIdentificationRequest inputCsvIdentificationRequest = randomCsvIdentificationRequest;
            CsvIdentificationRequest storageCsvIdentificationRequest = inputCsvIdentificationRequest.DeepClone();
            CsvIdentificationRequest expectedCsvIdentificationRequest = storageCsvIdentificationRequest.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedCsvIdentificationRequest);

            var expectedActionResult =
                new ActionResult<CsvIdentificationRequest>(expectedObjectResult);

            csvIdentificationRequestServiceMock
                .Setup(service => service.ModifyCsvIdentificationRequestAsync(inputCsvIdentificationRequest))
                    .ReturnsAsync(storageCsvIdentificationRequest);

            // when
            ActionResult<CsvIdentificationRequest> actualActionResult = await csvIdentificationRequestsController
                .PutCsvIdentificationRequestAsync(randomCsvIdentificationRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            csvIdentificationRequestServiceMock
               .Verify(service => service.ModifyCsvIdentificationRequestAsync(inputCsvIdentificationRequest),
                   Times.Once);

            csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }
    }
}
