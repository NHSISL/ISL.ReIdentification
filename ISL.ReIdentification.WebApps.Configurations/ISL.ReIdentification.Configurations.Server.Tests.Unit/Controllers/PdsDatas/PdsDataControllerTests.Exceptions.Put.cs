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

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.PdsDatas
{
    public partial class PdsDataControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            PdsData somePdsData = CreateRandomPdsData();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<PdsData>(expectedBadRequestObjectResult);

            this.pdsDataServiceMock.Setup(service =>
                service.ModifyPdsDataAsync(It.IsAny<PdsData>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<PdsData> actualActionResult =
                await this.pdsDataController.PutPdsDataAsync(somePdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.pdsDataServiceMock.Verify(service =>
                service.ModifyPdsDataAsync(It.IsAny<PdsData>()),
                    Times.Once);

            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            PdsData somePdsData = CreateRandomPdsData();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<PdsData>(expectedBadRequestObjectResult);

            this.pdsDataServiceMock.Setup(service =>
                service.ModifyPdsDataAsync(It.IsAny<PdsData>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<PdsData> actualActionResult =
                await this.pdsDataController.PutPdsDataAsync(somePdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.pdsDataServiceMock.Verify(service =>
                service.ModifyPdsDataAsync(It.IsAny<PdsData>()),
                    Times.Once);

            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            PdsData somePdsData = CreateRandomPdsData();
            string someMessage = GetRandomString();

            var notFoundPdsDataException =
                new NotFoundPdsDataException(
                    message: someMessage);

            var pdsDataValidationException =
                new PdsDataValidationException(
                    message: someMessage,
                    innerException: notFoundPdsDataException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundPdsDataException);

            var expectedActionResult =
                new ActionResult<PdsData>(expectedNotFoundObjectResult);

            this.pdsDataServiceMock.Setup(service =>
                service.ModifyPdsDataAsync(It.IsAny<PdsData>()))
                    .ThrowsAsync(pdsDataValidationException);

            // when
            ActionResult<PdsData> actualActionResult =
                await this.pdsDataController.PutPdsDataAsync(somePdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.pdsDataServiceMock.Verify(service =>
                service.ModifyPdsDataAsync(It.IsAny<PdsData>()),
                    Times.Once);

            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsPdsDataErrorOccurredAsync()
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
                service.ModifyPdsDataAsync(It.IsAny<PdsData>()))
                    .ThrowsAsync(pdsDataDependencyValidationException);

            // when
            ActionResult<PdsData> actualActionResult =
                await this.pdsDataController.PutPdsDataAsync(somePdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.pdsDataServiceMock.Verify(service =>
                service.ModifyPdsDataAsync(It.IsAny<PdsData>()),
                    Times.Once);

            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
