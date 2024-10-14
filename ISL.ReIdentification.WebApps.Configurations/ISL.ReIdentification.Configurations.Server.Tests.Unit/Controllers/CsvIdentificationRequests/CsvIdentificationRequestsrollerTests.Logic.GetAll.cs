// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            IQueryable<CsvIdentificationRequest> randomCsvIdentificationRequests =
                CreateRandomCsvIdentificationRequests();

            IQueryable<CsvIdentificationRequest> storageCsvIdentificationRequests =
                randomCsvIdentificationRequests.DeepClone();

            IQueryable<CsvIdentificationRequest> expectedCsvIdentificationRequest =
                storageCsvIdentificationRequests.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedCsvIdentificationRequest);

            var expectedActionResult =
                new ActionResult<IQueryable<CsvIdentificationRequest>>(expectedObjectResult);

            csvIdentificationRequestServiceMock
                .Setup(service => service.RetrieveAllCsvIdentificationRequestsAsync())
                    .ReturnsAsync(storageCsvIdentificationRequests);

            // when
            ActionResult<IQueryable<CsvIdentificationRequest>> actualActionResult =
                await csvIdentificationRequestsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            csvIdentificationRequestServiceMock
               .Verify(service => service.RetrieveAllCsvIdentificationRequestsAsync(),
                   Times.Once);

            csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }
    }
}
