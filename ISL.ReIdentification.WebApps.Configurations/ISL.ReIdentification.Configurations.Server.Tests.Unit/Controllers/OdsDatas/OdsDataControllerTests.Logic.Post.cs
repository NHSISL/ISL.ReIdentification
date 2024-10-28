// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.OdsDatas
{
    public partial class OdsDataControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            OdsData inputOdsData = randomOdsData;
            OdsData addedOdsData = inputOdsData.DeepClone();
            OdsData expectedOdsData = addedOdsData.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedOdsData);

            var expectedActionResult =
                new ActionResult<OdsData>(expectedObjectResult);

            odsDataServiceMock
                .Setup(service => service.AddOdsDataAsync(inputOdsData))
                    .ReturnsAsync(addedOdsData);

            // when
            ActionResult<OdsData> actualActionResult = await odsDataController.PostOdsDataAsync(randomOdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            odsDataServiceMock
               .Verify(service => service.AddOdsDataAsync(inputOdsData),
                   Times.Once);

            odsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}