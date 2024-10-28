// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            PdsData randomPdsData = CreateRandomPdsData();
            PdsData inputPdsData = randomPdsData;
            PdsData storagePdsData = inputPdsData.DeepClone();
            PdsData expectedPdsData = storagePdsData.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedPdsData);

            var expectedActionResult =
                new ActionResult<PdsData>(expectedObjectResult);

            pdsDataServiceMock
                .Setup(service => service.ModifyPdsDataAsync(inputPdsData))
                    .ReturnsAsync(storagePdsData);

            // when
            ActionResult<PdsData> actualActionResult = await pdsDataController.PutPdsDataAsync(randomPdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            pdsDataServiceMock
               .Verify(service => service.ModifyPdsDataAsync(inputPdsData),
                   Times.Once);

            pdsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
