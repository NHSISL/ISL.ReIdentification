// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.PdsDatas
{
    public partial class PdsDataControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            IQueryable<PdsData> somePdsDatas = CreateRandomPdsDatas();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<PdsData>>(expectedInternalServerErrorObjectResult);

            this.pdsDataServiceMock.Setup(service =>
                service.RetrieveAllPdsDatasAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<PdsData>> actualActionResult =
                await this.pdsDataController.GetAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.pdsDataServiceMock.Verify(service =>
                service.RetrieveAllPdsDatasAsync(),
                    Times.Once);

            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
