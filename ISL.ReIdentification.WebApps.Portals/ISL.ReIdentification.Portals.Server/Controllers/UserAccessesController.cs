// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Attrify.Attributes;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.UserAccesses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Portals.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserAccessesController : RESTFulController
    {
        private readonly IUserAccessProcessingService userAccessProcessingService;

        public UserAccessesController(IUserAccessProcessingService userAccessProcessingService) =>
            this.userAccessProcessingService = userAccessProcessingService;

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
        [InvisibleApi]
        [HttpPost]
        public async ValueTask<ActionResult<UserAccess>> PostUserAccessAsync(UserAccess userAccess)
        {
            try
            {
                UserAccess addedUserAccess =
                    await this.userAccessProcessingService.AddUserAccessAsync(userAccess);

                return Created(addedUserAccess);
            }
            catch (UserAccessProcessingValidationException userAccessProcessingValidationException)
            {
                return BadRequest(userAccessProcessingValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyValidationException lookupDependencyValidationException)
               when (lookupDependencyValidationException.InnerException is AlreadyExistsUserAccessException)
            {
                return Conflict(lookupDependencyValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyValidationException lookupDependencyValidationException)
            {
                return BadRequest(lookupDependencyValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyException lookupDependencyException)
            {
                return InternalServerError(lookupDependencyException);
            }
            catch (UserAccessProcessingServiceException lookupServiceException)
            {
                return InternalServerError(lookupServiceException);
            }
        }

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
        [InvisibleApi]
        [HttpPost("bulk")]
        public async ValueTask<ActionResult>
            PostBulkUserAccessAsync([FromBody] BulkUserAccess bulkUserAccess)
        {
            try
            {
                await this.userAccessProcessingService.BulkAddRemoveUserAccessAsync(bulkUserAccess);

                return Created();
            }
            catch (UserAccessProcessingValidationException userAccessProcessingValidationException)
            {
                return BadRequest(userAccessProcessingValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyValidationException lookupDependencyValidationException)
            {
                return BadRequest(lookupDependencyValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyException lookupDependencyException)
            {
                return InternalServerError(lookupDependencyException);
            }
            catch (UserAccessProcessingServiceException lookupServiceException)
            {
                return InternalServerError(lookupServiceException);
            }
        }

        [HttpGet]
        [EnableQuery(PageSize = 25)]
        public async ValueTask<ActionResult<IQueryable<UserAccess>>> Get()
        {
            try
            {
                IQueryable<UserAccess> userAccesses =
                    await this.userAccessProcessingService.RetrieveAllUserAccessesAsync();

                return Ok(userAccesses);
            }
            catch (UserAccessProcessingDependencyException userAccessProcessingDependencyException)
            {
                return InternalServerError(userAccessProcessingDependencyException);
            }
            catch (UserAccessProcessingServiceException userAccessProcessingServiceException)
            {
                return InternalServerError(userAccessProcessingServiceException);
            }
        }

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
        [InvisibleApi]
        [HttpGet("{userAccessId}")]
        public async ValueTask<ActionResult<UserAccess>> GetUserAccessByIdAsync(Guid userAccessId)
        {
            try
            {
                UserAccess userAccess =
                    await this.userAccessProcessingService.RetrieveUserAccessByIdAsync(userAccessId);

                return Ok(userAccess);
            }
            catch (UserAccessProcessingValidationException userAccessProcessingValidationException)
            {
                return BadRequest(userAccessProcessingValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyValidationException userAccessProcessingDependencyValidationException)
                when (userAccessProcessingDependencyValidationException.InnerException is NotFoundUserAccessException)
            {
                return NotFound(userAccessProcessingDependencyValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyValidationException userAccessProcessingDependencyValidationException)
            {
                return BadRequest(userAccessProcessingDependencyValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyException userAccessProcessingDependencyException)
            {
                return InternalServerError(userAccessProcessingDependencyException);
            }
            catch (UserAccessProcessingServiceException userAccessProcessingServiceException)
            {
                return InternalServerError(userAccessProcessingServiceException);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
        [InvisibleApi]
        [HttpPut]
        public async ValueTask<ActionResult<UserAccess>> PutUserAccessAsync(UserAccess userAccess)
        {
            try
            {
                UserAccess modifiedUserAccess =
                    await this.userAccessProcessingService.ModifyUserAccessAsync(userAccess);

                return Ok(modifiedUserAccess);
            }
            catch (UserAccessProcessingValidationException userAccessProcessingValidationException)
                when (userAccessProcessingValidationException.InnerException
                    is NotFoundUserAccessException)
            {
                return NotFound(userAccessProcessingValidationException.InnerException);
            }
            catch (UserAccessProcessingValidationException userAccessProcessingValidationException)
            {
                return BadRequest(userAccessProcessingValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyValidationException userAccessProcessingDependencyValidationException)
                when (userAccessProcessingDependencyValidationException.InnerException
                    is AlreadyExistsUserAccessException)
            {
                return Conflict(userAccessProcessingDependencyValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyValidationException userAccessProcessingDependencyValidationException)
            {
                return BadRequest(userAccessProcessingDependencyValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyException userAccessProcessingDependencyException)
            {
                return InternalServerError(userAccessProcessingDependencyException);
            }
            catch (UserAccessProcessingServiceException userAccessProcessingServiceException)
            {
                return InternalServerError(userAccessProcessingServiceException);
            }
        }

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
        [InvisibleApi]
        [HttpDelete("{userAccessId}")]
        public async ValueTask<ActionResult<UserAccess>> DeleteUserAccessByIdAsync(Guid userAccessId)
        {
            try
            {
                UserAccess deletedUserAccess =
                    await this.userAccessProcessingService.RemoveUserAccessByIdAsync(userAccessId);

                return Ok(deletedUserAccess);
            }
            catch (UserAccessProcessingValidationException userAccessProcessingValidationException)
                when (userAccessProcessingValidationException.InnerException
                    is NotFoundUserAccessException)
            {
                return NotFound(userAccessProcessingValidationException.InnerException);
            }
            catch (UserAccessProcessingValidationException userAccessProcessingValidationException)
            {
                return BadRequest(userAccessProcessingValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyValidationException userAccessProcessingDependencyValidationException)
                when (userAccessProcessingDependencyValidationException.InnerException is LockedUserAccessException)
            {
                return Locked(userAccessProcessingDependencyValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyValidationException userAccessProcessingDependencyValidationException)
            {
                return BadRequest(userAccessProcessingDependencyValidationException.InnerException);
            }
            catch (UserAccessProcessingDependencyException userAccessProcessingDependencyException)
            {
                return InternalServerError(userAccessProcessingDependencyException);
            }
            catch (UserAccessProcessingServiceException userAccessProcessingServiceException)
            {
                return InternalServerError(userAccessProcessingServiceException);
            }
        }
    }
}

