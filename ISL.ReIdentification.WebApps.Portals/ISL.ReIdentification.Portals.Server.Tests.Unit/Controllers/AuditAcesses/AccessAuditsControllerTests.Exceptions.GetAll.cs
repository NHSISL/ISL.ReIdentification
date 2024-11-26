// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.AccessAudits
{
    public partial class AccessAuditsControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            IQueryable<AccessAudit> someAccessAudits = CreateRandomAccessAudits();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<AccessAudit>>(expectedInternalServerErrorObjectResult);

            this.accessAuditServiceMock.Setup(service =>
                service.RetrieveAllAccessAuditsAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<AccessAudit>> actualActionResult =
                await this.accessAuditsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.accessAuditServiceMock.Verify(service =>
                service.RetrieveAllAccessAuditsAsync(),
                    Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
        }
    }
}
