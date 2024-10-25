// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.PdsDatas
{
    public partial class PdsDataControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            IQueryable<PdsData> randomPdsDatas = CreateRandomPdsDatas();
            IQueryable<PdsData> storagePdsDatas = randomPdsDatas.DeepClone();
            IQueryable<PdsData> expectedPdsData = storagePdsDatas.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedPdsData);

            var expectedActionResult =
                new ActionResult<IQueryable<PdsData>>(expectedObjectResult);

            pdsDataServiceMock
                .Setup(service => service.RetrieveAllPdsDatasAsync())
                    .ReturnsAsync(storagePdsDatas);

            // when
            ActionResult<IQueryable<PdsData>> actualActionResult = await pdsDataController.GetAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            pdsDataServiceMock
               .Verify(service => service.RetrieveAllPdsDatasAsync(),
                   Times.Once);

            pdsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
