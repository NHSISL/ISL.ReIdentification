// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Services.Processings.UserAccesses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Configurations.Server.Controllers
{
    [Authorize(Roles = "ISL.Reidentification.Configuration.Administrators,ISL.Reidentification.Configuration.Users")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserAccessesController : RESTFulController
    {
        private readonly IUserAccessProcessingService userAccessProcessingService;

        public UserAccessesController(IUserAccessProcessingService userAccessProcessingService) =>
            this.userAccessProcessingService = userAccessProcessingService;

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

        [HttpPost("bulk")]
        public async ValueTask<ActionResult> PostBulkUserAccessAsync([FromBody] BulkUserAccess bulkUserAccess)
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
#if !DEBUG
        [EnableQuery(PageSize = 25)]
#endif
#if DEBUG
        [EnableQuery(PageSize = 5000)]
#endif
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
