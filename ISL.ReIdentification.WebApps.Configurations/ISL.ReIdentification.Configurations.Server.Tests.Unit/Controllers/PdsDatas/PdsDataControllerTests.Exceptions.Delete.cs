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
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<PdsData>(expectedBadRequestObjectResult);

            this.pdsDataServiceMock.Setup(service =>
                service.RemovePdsDataByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<PdsData> actualActionResult =
                await this.pdsDataController.DeletePdsDataByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.pdsDataServiceMock.Verify(service =>
                service.RemovePdsDataByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.pdsDataServiceMock.VerifyNoOtherCalls();
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
                new ActionResult<PdsData>(expectedBadRequestObjectResult);

            this.pdsDataServiceMock.Setup(service =>
                service.RemovePdsDataByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<PdsData> actualActionResult =
                await this.pdsDataController.DeletePdsDataByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.pdsDataServiceMock.Verify(service =>
                service.RemovePdsDataByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
                service.RemovePdsDataByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(pdsDataValidationException);

            // when
            ActionResult<PdsData> actualActionResult =
                await this.pdsDataController.DeletePdsDataByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.pdsDataServiceMock.Verify(service =>
                service.RemovePdsDataByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfRecordIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var lockedPdsDataException =
                new LockedPdsDataException(
                    message: someMessage,
                    innerException: someInnerException);

            var pdsDataDependencyValidationException =
                new PdsDataDependencyValidationException(
                    message: someMessage,
                    innerException: lockedPdsDataException);

            LockedObjectResult expectedConflictObjectResult =
                Locked(lockedPdsDataException);

            var expectedActionResult =
                new ActionResult<PdsData>(expectedConflictObjectResult);

            this.pdsDataServiceMock.Setup(service =>
                service.RemovePdsDataByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(pdsDataDependencyValidationException);

            // when
            ActionResult<PdsData> actualActionResult =
                await this.pdsDataController.DeletePdsDataByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.pdsDataServiceMock.Verify(service =>
                service.RemovePdsDataByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
