// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.OdsDatas
{
    public partial class OdsDataControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            OdsData inputOdsData = randomOdsData;
            OdsData storageOdsData = inputOdsData.DeepClone();
            OdsData expectedOdsData = storageOdsData.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedOdsData);

            var expectedActionResult =
                new ActionResult<OdsData>(expectedObjectResult);

            odsDataServiceMock
                .Setup(service => service.ModifyOdsDataAsync(inputOdsData))
                    .ReturnsAsync(storageOdsData);

            // when
            ActionResult<OdsData> actualActionResult = await odsDataController.PutOdsDataAsync(randomOdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            odsDataServiceMock
               .Verify(service => service.ModifyOdsDataAsync(inputOdsData),
                   Times.Once);

            odsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
