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
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<OdsData>(expectedBadRequestObjectResult);

            this.odsDataServiceMock.Setup(service =>
                service.RemoveOdsDataByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<OdsData> actualActionResult =
                await this.odsDataController.DeleteOdsDataByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.odsDataServiceMock.Verify(service =>
                service.RemoveOdsDataByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.odsDataServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnDeleteIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<OdsData>(expectedBadRequestObjectResult);

            this.odsDataServiceMock.Setup(service =>
                service.RemoveOdsDataByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<OdsData> actualActionResult =
                await this.odsDataController.DeleteOdsDataByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.odsDataServiceMock.Verify(service =>
                service.RemoveOdsDataByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.odsDataServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
                service.RemoveOdsDataByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(odsDataValidationException);

            // when
            ActionResult<OdsData> actualActionResult =
                await this.odsDataController.DeleteOdsDataByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.odsDataServiceMock.Verify(service =>
                service.RemoveOdsDataByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.odsDataServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfRecordIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var lockedOdsDataException =
                new LockedOdsDataException(
                    message: someMessage,
                    innerException: someInnerException);

            var odsDataDependencyValidationException =
                new OdsDataDependencyValidationException(
                    message: someMessage,
                    innerException: lockedOdsDataException);

            LockedObjectResult expectedConflictObjectResult =
                Locked(lockedOdsDataException);

            var expectedActionResult =
                new ActionResult<OdsData>(expectedConflictObjectResult);

            this.odsDataServiceMock.Setup(service =>
                service.RemoveOdsDataByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(odsDataDependencyValidationException);

            // when
            ActionResult<OdsData> actualActionResult =
                await this.odsDataController.DeleteOdsDataByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.odsDataServiceMock.Verify(service =>
                service.RemoveOdsDataByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.odsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
