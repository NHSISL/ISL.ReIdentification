// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            IQueryable<CsvIdentificationRequest> someCsvIdentificationRequests =
                CreateRandomCsvIdentificationRequests();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<CsvIdentificationRequest>>(expectedInternalServerErrorObjectResult);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.RetrieveAllCsvIdentificationRequestsAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<CsvIdentificationRequest>> actualActionResult =
                await this.csvIdentificationRequestsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RetrieveAllCsvIdentificationRequestsAsync(),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }
    }
}
