// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.AccessAudits
{
    public partial class AccessAuditsControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            AccessAudit randomAccessAudit = CreateRandomAccessAudit();
            AccessAudit inputAccessAudit = randomAccessAudit;
            AccessAudit storageAccessAudit = inputAccessAudit.DeepClone();
            AccessAudit expectedAccessAudit = storageAccessAudit.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedAccessAudit);

            var expectedActionResult =
                new ActionResult<AccessAudit>(expectedObjectResult);

            accessAuditServiceMock
                .Setup(service => service.ModifyAccessAuditAsync(inputAccessAudit))
                    .ReturnsAsync(storageAccessAudit);

            // when
            ActionResult<AccessAudit> actualActionResult = await accessAuditsController.PutAccessAuditAsync(randomAccessAudit);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            accessAuditServiceMock
               .Verify(service => service.ModifyAccessAuditAsync(inputAccessAudit),
                   Times.Once);

            accessAuditServiceMock.VerifyNoOtherCalls();
        }
    }
}
