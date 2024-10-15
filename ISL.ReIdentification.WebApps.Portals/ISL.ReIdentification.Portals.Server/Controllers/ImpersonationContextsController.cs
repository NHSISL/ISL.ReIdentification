// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Portals.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImpersonationContextsController : RESTFulController
    {
        private readonly IImpersonationContextService impersonationContextService;

        public ImpersonationContextsController(IImpersonationContextService impersonationContextService) =>
            this.impersonationContextService = impersonationContextService;

        [HttpPost]
        public async ValueTask<ActionResult<ImpersonationContext>> PostImpersonationContextAsync(
            [FromBody] ImpersonationContext impersonationContext)
        {
            try
            {
                ImpersonationContext addedImpersonationContext =
                    await impersonationContextService.AddImpersonationContextAsync(impersonationContext);

                return Created(addedImpersonationContext);
            }
            catch (ImpersonationContextValidationException lookupValidationException)
            {
                return BadRequest(lookupValidationException.InnerException);
            }
            catch (ImpersonationContextDependencyValidationException lookupDependencyValidationException)
               when (lookupDependencyValidationException.InnerException is AlreadyExistsImpersonationContextException)
            {
                return Conflict(lookupDependencyValidationException.InnerException);
            }
            catch (ImpersonationContextDependencyValidationException lookupDependencyValidationException)
            {
                return BadRequest(lookupDependencyValidationException.InnerException);
            }
            catch (ImpersonationContextDependencyException lookupDependencyException)
            {
                return InternalServerError(lookupDependencyException);
            }
            catch (ImpersonationContextServiceException lookupServiceException)
            {
                return InternalServerError(lookupServiceException);
            }
        }

        [HttpGet]
#if !DEBUG
        [EnableQuery(PageSize = 50)]
#endif
#if DEBUG
        [EnableQuery(PageSize = 25)]
#endif
#if RELEASE
        [Authorize(Roles = "")]
#endif
        public async ValueTask<ActionResult<IQueryable<ImpersonationContext>>> Get()
        {
            try
            {
                IQueryable<ImpersonationContext> impersonationContexts =
                    await impersonationContextService.RetrieveAllImpersonationContextsAsync();

                return Ok(impersonationContexts);
            }
            catch (ImpersonationContextDependencyException impersonationContextDependencyException)
            {
                return InternalServerError(impersonationContextDependencyException);
            }
            catch (ImpersonationContextServiceException impersonationContextServiceException)
            {
                return InternalServerError(impersonationContextServiceException);
            }
        }

        [HttpGet("{impersonationContextId}")]
        public async ValueTask<ActionResult<ImpersonationContext>> GetImpersonationContextByIdAsync(Guid impersonationContextId)
        {
            try
            {
                ImpersonationContext impersonationContext =
                    await impersonationContextService.RetrieveImpersonationContextByIdAsync(impersonationContextId);

                return Ok(impersonationContext);
            }
            catch (ImpersonationContextValidationException impersonationContextValidationException)
                when (impersonationContextValidationException.InnerException is NotFoundImpersonationContextException)
            {
                return NotFound(impersonationContextValidationException.InnerException);
            }
            catch (ImpersonationContextValidationException impersonationContextValidationException)
            {
                return BadRequest(impersonationContextValidationException.InnerException);
            }
            catch (ImpersonationContextDependencyValidationException impersonationContextDependencyValidationException)
            {
                return BadRequest(impersonationContextDependencyValidationException.InnerException);
            }
            catch (ImpersonationContextDependencyException impersonationContextDependencyException)
            {
                return InternalServerError(impersonationContextDependencyException);
            }
            catch (ImpersonationContextServiceException impersonationContextServiceException)
            {
                return InternalServerError(impersonationContextServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<ImpersonationContext>> PutImpersonationContextAsync(
            [FromBody] ImpersonationContext impersonationContext)
        {
            try
            {
                ImpersonationContext modifiedImpersonationContext =
                    await impersonationContextService.ModifyImpersonationContextAsync(impersonationContext);

                return Ok(modifiedImpersonationContext);
            }
            catch (ImpersonationContextValidationException impersonationContextValidationException)
                when (impersonationContextValidationException.InnerException
                    is NotFoundImpersonationContextException)
            {
                return NotFound(impersonationContextValidationException.InnerException);
            }
            catch (ImpersonationContextValidationException impersonationContextValidationException)
            {
                return BadRequest(impersonationContextValidationException.InnerException);
            }
            catch (ImpersonationContextDependencyValidationException impersonationContextDependencyValidationException)
                when (impersonationContextDependencyValidationException.InnerException
                    is AlreadyExistsImpersonationContextException)
            {
                return Conflict(impersonationContextDependencyValidationException.InnerException);
            }
            catch (ImpersonationContextDependencyValidationException impersonationContextDependencyValidationException)
            {
                return BadRequest(impersonationContextDependencyValidationException.InnerException);
            }
            catch (ImpersonationContextDependencyException impersonationContextDependencyException)
            {
                return InternalServerError(impersonationContextDependencyException);
            }
            catch (ImpersonationContextServiceException impersonationContextServiceException)
            {
                return InternalServerError(impersonationContextServiceException);
            }
        }

        [HttpDelete("{impersonationContextId}")]
        public async ValueTask<ActionResult<ImpersonationContext>> DeleteImpersonationContextByIdAsync(Guid impersonationContextId)
        {
            try
            {
                ImpersonationContext deletedImpersonationContext =
                    await impersonationContextService.RemoveImpersonationContextByIdAsync(impersonationContextId);

                return Ok(deletedImpersonationContext);
            }
            catch (ImpersonationContextValidationException impersonationContextValidationException)
                when (impersonationContextValidationException.InnerException
                    is NotFoundImpersonationContextException)
            {
                return NotFound(impersonationContextValidationException.InnerException);
            }
            catch (ImpersonationContextValidationException impersonationContextValidationException)
            {
                return BadRequest(impersonationContextValidationException.InnerException);
            }
            catch (ImpersonationContextDependencyValidationException impersonationContextDependencyValidationException)
                when (impersonationContextDependencyValidationException.InnerException is LockedImpersonationContextException)
            {
                return Locked(impersonationContextDependencyValidationException.InnerException);
            }
            catch (ImpersonationContextDependencyValidationException impersonationContextDependencyValidationException)
            {
                return BadRequest(impersonationContextDependencyValidationException.InnerException);
            }
            catch (ImpersonationContextDependencyException impersonationContextDependencyException)
            {
                return InternalServerError(impersonationContextDependencyException);
            }
            catch (ImpersonationContextServiceException impersonationContextServiceException)
            {
                return InternalServerError(impersonationContextServiceException);
            }
        }
    }
}
