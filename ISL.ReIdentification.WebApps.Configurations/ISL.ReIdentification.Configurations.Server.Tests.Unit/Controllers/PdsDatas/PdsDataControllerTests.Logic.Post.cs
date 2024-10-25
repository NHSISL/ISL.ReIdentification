// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.PdsDatas
{
    public partial class PdsDataControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            PdsData randomPdsData = CreateRandomPdsData();
            PdsData inputPdsData = randomPdsData;
            PdsData addedPdsData = inputPdsData.DeepClone();
            PdsData expectedPdsData = addedPdsData.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedPdsData);

            var expectedActionResult =
                new ActionResult<PdsData>(expectedObjectResult);

            pdsDataServiceMock
                .Setup(service => service.AddPdsDataAsync(inputPdsData))
                    .ReturnsAsync(addedPdsData);

            // when
            ActionResult<PdsData> actualActionResult = await pdsDataController.PostPdsDataAsync(randomPdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            pdsDataServiceMock
               .Verify(service => service.AddPdsDataAsync(inputPdsData),
                   Times.Once);

            pdsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}