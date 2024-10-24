// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.PdsDatas
{
    public partial class PdsDataControllerTests
    {
        [Fact]
        public async Task DeletePdsDataByIdAsyncShouldReturnPdsData()
        {
            // given
            PdsData randomPdsData = CreateRandomPdsData();
            Guid inputId = randomPdsData.Id;
            PdsData storagePdsData = randomPdsData;
            PdsData expectedPdsData = storagePdsData.DeepClone();

            pdsDataServiceMock.Setup(service =>
                service.RemovePdsDataByIdAsync(inputId))
                .ReturnsAsync(storagePdsData);

            // when
            var result = await this.pdsDataController.DeletePdsDataByIdAsync(inputId);

            // then
            var actualResult = Assert.IsType<OkObjectResult>(result.Result);
            actualResult.StatusCode.Should().Be(200);
            actualResult.Value.Should().BeEquivalentTo(expectedPdsData);
        }

        [Fact]
        public async Task
            DeletePdsDataByIdAsyncShouldReturnNotFoundWhenPdsDataValidationExceptionOccurs()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputId = randomId;
            var notFoundPdsDataException = new NotFoundPdsDataException(message: inputId.ToString());

            var delegatedAccessValidationException = new PdsDataValidationException(
                message: GetRandomString(),
                innerException: notFoundPdsDataException);

            pdsDataServiceMock
                .Setup(service => service.RemovePdsDataByIdAsync(inputId))
                .ThrowsAsync(delegatedAccessValidationException);

            // when
            var result = await pdsDataController.DeletePdsDataByIdAsync(inputId);

            // then
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            notFoundObjectResult.StatusCode.Should().Be(404);
        }
    }
}
