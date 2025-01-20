// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.AccessAudits
{
    public partial class AccessAuditsControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            AccessAudit randomAccessAudit = CreateRandomAccessAudit();
            AccessAudit inputAccessAudit = randomAccessAudit;
            AccessAudit addedAccessAudit = inputAccessAudit.DeepClone();
            AccessAudit expectedAccessAudit = addedAccessAudit.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedAccessAudit);

            var expectedActionResult =
                new ActionResult<AccessAudit>(expectedObjectResult);

            accessAuditServiceMock
                .Setup(service => service.AddAccessAuditAsync(inputAccessAudit))
                    .ReturnsAsync(addedAccessAudit);

            // when
            ActionResult<AccessAudit> actualActionResult = await accessAuditsController.PostAccessAuditAsync(randomAccessAudit);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            accessAuditServiceMock
               .Verify(service => service.AddAccessAuditAsync(inputAccessAudit),
                   Times.Once);

            accessAuditServiceMock.VerifyNoOtherCalls();
        }
    }
}