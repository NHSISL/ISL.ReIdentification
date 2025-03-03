// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
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
            ApprovalRequest randomApprovalRequest = CreateRandomApprovalRequest();
            ApprovalRequest inputApprovalRequest = randomApprovalRequest;
            var expectedResult = new OkResult();
            var expectedActionResult = expectedResult;

            // when
            ActionResult actualActionResult = await reIdentificationController
                .PostImpersonationContextApprovalAsync(inputApprovalRequest);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            identificationCoordinationServiceMock
                .Verify(service => service.ImpersonationContextApprovalAsync(
                    inputApprovalRequest.ImpersonationContextId,
                    inputApprovalRequest.IsApproved),
                        Times.Once);

            identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
