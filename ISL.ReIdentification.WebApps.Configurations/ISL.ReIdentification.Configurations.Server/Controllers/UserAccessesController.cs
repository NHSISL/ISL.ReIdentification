// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ISL.Reidentification.Core.Models.Foundations.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.UserAccesses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Configurations.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAccessesController : RESTFulController
    {
        private readonly IUserAccessService userAccessService;

        public UserAccessesController(IUserAccessService userAccessService) =>
            this.userAccessService = userAccessService;

        [HttpPost]
        public async ValueTask<ActionResult<UserAccess>> PostUserAccessAsync(UserAccess userAccess)
        {
            try
            {
                UserAccess addedUserAccess =
                    await this.userAccessService.AddUserAccessAsync(userAccess);

                return Created(addedUserAccess);
            }
            catch (UserAccessValidationException userAccessValidationException)
            {
                return BadRequest(userAccessValidationException.InnerException);
            }
            catch (UserAccessDependencyValidationException lookupDependencyValidationException)
               when (lookupDependencyValidationException.InnerException is AlreadyExistsUserAccessException)
            {
                return Conflict(lookupDependencyValidationException.InnerException);
            }
            catch (UserAccessDependencyValidationException lookupDependencyValidationException)
            {
                return BadRequest(lookupDependencyValidationException.InnerException);
            }
            catch (UserAccessDependencyException lookupDependencyException)
            {
                return InternalServerError(lookupDependencyException);
            }
            catch (UserAccessServiceException lookupServiceException)
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
        public async ValueTask<ActionResult<IQueryable<UserAccess>>> Get()
        {
            try
            {
                IQueryable<UserAccess> userAccesses =
                    await this.userAccessService.RetrieveAllUserAccessesAsync();

                return Ok(userAccesses);
            }
            catch (UserAccessDependencyException userAccessDependencyException)
            {
                return InternalServerError(userAccessDependencyException);
            }
            catch (UserAccessServiceException userAccessServiceException)
            {
                return InternalServerError(userAccessServiceException);
            }
        }

        [HttpGet("{userAccessId}")]
        public async ValueTask<ActionResult<UserAccess>> GetUserAccessByIdAsync(Guid userAccessId)
        {
            try
            {
                UserAccess userAccess =
                    await this.userAccessService.RetrieveUserAccessByIdAsync(userAccessId);

                return Ok(userAccess);
            }
            catch (UserAccessValidationException userAccessValidationException)
                when (userAccessValidationException.InnerException is NotFoundUserAccessException)
            {
                return NotFound(userAccessValidationException.InnerException);
            }
            catch (UserAccessValidationException userAccessValidationException)
            {
                return BadRequest(userAccessValidationException.InnerException);
            }
            catch (UserAccessDependencyValidationException userAccessDependencyValidationException)
            {
                return BadRequest(userAccessDependencyValidationException.InnerException);
            }
            catch (UserAccessDependencyException userAccessDependencyException)
            {
                return InternalServerError(userAccessDependencyException);
            }
            catch (UserAccessServiceException userAccessServiceException)
            {
                return InternalServerError(userAccessServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<UserAccess>> PutUserAccessAsync(UserAccess userAccess)
        {
            try
            {
                UserAccess modifiedUserAccess =
                    await this.userAccessService.ModifyUserAccessAsync(userAccess);

                return Ok(modifiedUserAccess);
            }
            catch (UserAccessValidationException userAccessValidationException)
                when (userAccessValidationException.InnerException
                    is NotFoundUserAccessException)
            {
                return NotFound(userAccessValidationException.InnerException);
            }
            catch (UserAccessValidationException userAccessValidationException)
            {
                return BadRequest(userAccessValidationException.InnerException);
            }
            catch (UserAccessDependencyValidationException userAccessDependencyValidationException)
                when (userAccessDependencyValidationException.InnerException
                    is AlreadyExistsUserAccessException)
            {
                return Conflict(userAccessDependencyValidationException.InnerException);
            }
            catch (UserAccessDependencyValidationException userAccessDependencyValidationException)
            {
                return BadRequest(userAccessDependencyValidationException.InnerException);
            }
            catch (UserAccessDependencyException userAccessDependencyException)
            {
                return InternalServerError(userAccessDependencyException);
            }
            catch (UserAccessServiceException userAccessServiceException)
            {
                return InternalServerError(userAccessServiceException);
            }
        }

        [HttpDelete("{userAccessId}")]
        public async ValueTask<ActionResult<UserAccess>> DeleteUserAccessByIdAsync(Guid userAccessId)
        {
            try
            {
                UserAccess deletedUserAccess =
                    await this.userAccessService.RemoveUserAccessByIdAsync(userAccessId);

                return Ok(deletedUserAccess);
            }
            catch (UserAccessValidationException userAccessValidationException)
                when (userAccessValidationException.InnerException
                    is NotFoundUserAccessException)
            {
                return NotFound(userAccessValidationException.InnerException);
            }
            catch (UserAccessValidationException userAccessValidationException)
            {
                return BadRequest(userAccessValidationException.InnerException);
            }
            catch (UserAccessDependencyValidationException userAccessDependencyValidationException)
                when (userAccessDependencyValidationException.InnerException is LockedUserAccessException)
            {
                return Locked(userAccessDependencyValidationException.InnerException);
            }
            catch (UserAccessDependencyValidationException userAccessDependencyValidationException)
            {
                return BadRequest(userAccessDependencyValidationException.InnerException);
            }
            catch (UserAccessDependencyException userAccessDependencyException)
            {
                return InternalServerError(userAccessDependencyException);
            }
            catch (UserAccessServiceException userAccessServiceException)
            {
                return InternalServerError(userAccessServiceException);
            }
        }
    }
}
