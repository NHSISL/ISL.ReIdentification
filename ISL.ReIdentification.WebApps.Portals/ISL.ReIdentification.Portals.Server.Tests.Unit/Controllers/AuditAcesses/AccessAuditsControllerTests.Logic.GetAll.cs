// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.AccessAudits
{
    public partial class AccessAuditsControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            IQueryable<AccessAudit> randomAccessAudits = CreateRandomAccessAudits();
            IQueryable<AccessAudit> storageAccessAudits = randomAccessAudits.DeepClone();
            IQueryable<AccessAudit> expectedAccessAudit = storageAccessAudits.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedAccessAudit);

            var expectedActionResult =
                new ActionResult<IQueryable<AccessAudit>>(expectedObjectResult);

            accessAuditServiceMock
                .Setup(service => service.RetrieveAllAccessAuditsAsync())
                    .ReturnsAsync(storageAccessAudits);

            // when
            ActionResult<IQueryable<AccessAudit>> actualActionResult = await accessAuditsController.GetAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            accessAuditServiceMock
               .Verify(service => service.RetrieveAllAccessAuditsAsync(),
                   Times.Once);

            accessAuditServiceMock.VerifyNoOtherCalls();
        }
    }
}
