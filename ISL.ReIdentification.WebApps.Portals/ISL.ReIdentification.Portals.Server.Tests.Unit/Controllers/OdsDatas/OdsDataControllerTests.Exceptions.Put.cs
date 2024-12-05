// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.OdsDatas
{
    public partial class OdsDataControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            OdsData someOdsData = CreateRandomOdsData();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<OdsData>(expectedBadRequestObjectResult);

            this.odsDataServiceMock.Setup(service =>
                service.ModifyOdsDataAsync(It.IsAny<OdsData>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<OdsData> actualActionResult =
                await this.odsDataController.PutOdsDataAsync(someOdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.odsDataServiceMock.Verify(service =>
                service.ModifyOdsDataAsync(It.IsAny<OdsData>()),
                    Times.Once);

            this.odsDataServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            OdsData someOdsData = CreateRandomOdsData();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<OdsData>(expectedBadRequestObjectResult);

            this.odsDataServiceMock.Setup(service =>
                service.ModifyOdsDataAsync(It.IsAny<OdsData>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<OdsData> actualActionResult =
                await this.odsDataController.PutOdsDataAsync(someOdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.odsDataServiceMock.Verify(service =>
                service.ModifyOdsDataAsync(It.IsAny<OdsData>()),
                    Times.Once);

            this.odsDataServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            OdsData someOdsData = CreateRandomOdsData();
            string someMessage = GetRandomString();

            var notFoundOdsDataException =
                new NotFoundOdsDataException(
                    message: someMessage);

            var odsDataValidationException =
                new OdsDataValidationException(
                    message: someMessage,
                    innerException: notFoundOdsDataException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundOdsDataException);

            var expectedActionResult =
                new ActionResult<OdsData>(expectedNotFoundObjectResult);

            this.odsDataServiceMock.Setup(service =>
                service.ModifyOdsDataAsync(It.IsAny<OdsData>()))
                    .ThrowsAsync(odsDataValidationException);

            // when
            ActionResult<OdsData> actualActionResult =
                await this.odsDataController.PutOdsDataAsync(someOdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.odsDataServiceMock.Verify(service =>
                service.ModifyOdsDataAsync(It.IsAny<OdsData>()),
                    Times.Once);

            this.odsDataServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsOdsDataErrorOccurredAsync()
        {
            // given
            OdsData someOdsData = CreateRandomOdsData();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsOdsDataException =
                new AlreadyExistsOdsDataException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var odsDataDependencyValidationException =
                new OdsDataDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsOdsDataException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsOdsDataException);

            var expectedActionResult =
                new ActionResult<OdsData>(expectedConflictObjectResult);

            this.odsDataServiceMock.Setup(service =>
                service.ModifyOdsDataAsync(It.IsAny<OdsData>()))
                    .ThrowsAsync(odsDataDependencyValidationException);

            // when
            ActionResult<OdsData> actualActionResult =
                await this.odsDataController.PutOdsDataAsync(someOdsData);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.odsDataServiceMock.Verify(service =>
                service.ModifyOdsDataAsync(It.IsAny<OdsData>()),
                    Times.Once);

            this.odsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
