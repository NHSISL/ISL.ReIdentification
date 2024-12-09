// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.PdsDatas
{
    public partial class PdsDataControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            PdsData somePdsData = CreateRandomPdsData();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<PdsData>(expectedBadRequestObjectResult);

            this.pdsDataServiceMock.Setup(service =>
                service.AddPdsDataAsync(It.IsAny<PdsData>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<PdsData> actualActionResult =
                await this.pdsDataController.PostPdsDataAsync(somePdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.pdsDataServiceMock.Verify(service =>
                service.AddPdsDataAsync(It.IsAny<PdsData>()),
                    Times.Once);

            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            PdsData somePdsData = CreateRandomPdsData();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<PdsData>(expectedBadRequestObjectResult);

            this.pdsDataServiceMock.Setup(service =>
                service.AddPdsDataAsync(It.IsAny<PdsData>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<PdsData> actualActionResult =
                await this.pdsDataController.PostPdsDataAsync(somePdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.pdsDataServiceMock.Verify(service =>
                service.AddPdsDataAsync(It.IsAny<PdsData>()),
                    Times.Once);

            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsPdsDataErrorOccurredAsync()
        {
            // given
            PdsData somePdsData = CreateRandomPdsData();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsPdsDataException =
                new AlreadyExistsPdsDataException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var pdsDataDependencyValidationException =
                new PdsDataDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsPdsDataException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsPdsDataException);

            var expectedActionResult =
                new ActionResult<PdsData>(expectedConflictObjectResult);

            this.pdsDataServiceMock.Setup(service =>
                service.AddPdsDataAsync(It.IsAny<PdsData>()))
                    .ThrowsAsync(pdsDataDependencyValidationException);

            // when
            ActionResult<PdsData> actualActionResult =
                await this.pdsDataController.PostPdsDataAsync(somePdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.pdsDataServiceMock.Verify(service =>
                service.AddPdsDataAsync(It.IsAny<PdsData>()),
                    Times.Once);

            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}