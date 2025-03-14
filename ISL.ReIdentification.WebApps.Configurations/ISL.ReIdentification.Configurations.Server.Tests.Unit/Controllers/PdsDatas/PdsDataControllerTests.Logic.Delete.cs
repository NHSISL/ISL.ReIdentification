﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldRemoveRecordOnDeleteByIdsAsync()
        {
            // given
            PdsData randomPdsData = CreateRandomPdsData();
            Guid inputId = randomPdsData.Id;
            PdsData storagePdsData = randomPdsData;
            PdsData expectedPdsData = storagePdsData.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedPdsData);

            var expectedActionResult =
                new ActionResult<PdsData>(expectedObjectResult);

            pdsDataServiceMock
                .Setup(service => service.RemovePdsDataByIdAsync(inputId))
                    .ReturnsAsync(storagePdsData);

            // when
            ActionResult<PdsData> actualActionResult =
                await pdsDataController.DeletePdsDataByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            pdsDataServiceMock
                .Verify(service => service.RemovePdsDataByIdAsync(inputId),
                    Times.Once);

            pdsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
