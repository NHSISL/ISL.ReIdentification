// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.ReIdentification
{
    public partial class ReIdentificationControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPostPostImpersonationContextApprovalAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid impersonationContextId = randomId;
            bool isApproved = true;
            var expectedResult = new OkResult();
            var expectedActionResult = expectedResult;

            // when
            ActionResult actualActionResult = await reIdentificationController
                .PostImpersonationContextApprovalAsync(impersonationContextId, isApproved);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            identificationCoordinationServiceMock
                .Verify(service => service.ImpersonationContextApprovalAsync(
                    impersonationContextId,
                    isApproved),
                        Times.Once);

            identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
