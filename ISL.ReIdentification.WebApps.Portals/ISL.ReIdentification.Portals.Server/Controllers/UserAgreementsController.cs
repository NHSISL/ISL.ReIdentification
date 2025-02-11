// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Attrify.Attributes;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.UserAgreements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Portals.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserAgreementsController : RESTFulController
    {
        private readonly IUserAgreementService userAgreementService;

        public UserAgreementsController(IUserAgreementService userAgreementService) =>
            this.userAgreementService = userAgreementService;

        [HttpPost]
        public async ValueTask<ActionResult<UserAgreement>> PostUserAgreementAsync(UserAgreement userAgreement)
        {
            try
            {
                UserAgreement addedUserAgreement =
                    await this.userAgreementService.AddUserAgreementAsync(userAgreement);

                return Created(addedUserAgreement);
            }
            catch (UserAgreementValidationException userAgreementValidationException)
            {
                return BadRequest(userAgreementValidationException.InnerException);
            }
            catch (UserAgreementDependencyValidationException lookupDependencyValidationException)
               when (lookupDependencyValidationException.InnerException is AlreadyExistsUserAgreementException)
            {
                return Conflict(lookupDependencyValidationException.InnerException);
            }
            catch (UserAgreementDependencyValidationException lookupDependencyValidationException)
            {
                return BadRequest(lookupDependencyValidationException.InnerException);
            }
            catch (UserAgreementDependencyException lookupDependencyException)
            {
                return InternalServerError(lookupDependencyException);
            }
            catch (UserAgreementServiceException lookupServiceException)
            {
                return InternalServerError(lookupServiceException);
            }
        }

        [HttpGet]
#if !DEBUG
        [EnableQuery(PageSize = 50)]
#endif
#if DEBUG
        [EnableQuery(PageSize = 5000)]
#endif
        public async ValueTask<ActionResult<IQueryable<UserAgreement>>> Get()
        {
            try
            {
                IQueryable<UserAgreement> userAgreementes =
                    await this.userAgreementService.RetrieveAllUserAgreementsAsync();

                return Ok(userAgreementes);
            }
            catch (UserAgreementDependencyException userAgreementDependencyException)
            {
                return InternalServerError(userAgreementDependencyException);
            }
            catch (UserAgreementServiceException userAgreementServiceException)
            {
                return InternalServerError(userAgreementServiceException);
            }
        }

        [InvisibleApi]
        [HttpGet("{userAgreementId}")]
        public async ValueTask<ActionResult<UserAgreement>> GetUserAgreementByIdAsync(Guid userAgreementId)
        {
            try
            {
                UserAgreement userAgreement =
                    await this.userAgreementService.RetrieveUserAgreementByIdAsync(userAgreementId);

                return Ok(userAgreement);
            }
            catch (UserAgreementValidationException userAgreementValidationException)
            {
                return BadRequest(userAgreementValidationException.InnerException);
            }
            catch (UserAgreementDependencyValidationException userAgreementDependencyValidationException)
                when (userAgreementDependencyValidationException.InnerException is NotFoundUserAgreementException)
            {
                return NotFound(userAgreementDependencyValidationException.InnerException);
            }
            catch (UserAgreementDependencyValidationException userAgreementDependencyValidationException)
            {
                return BadRequest(userAgreementDependencyValidationException.InnerException);
            }
            catch (UserAgreementDependencyException userAgreementDependencyException)
            {
                return InternalServerError(userAgreementDependencyException);
            }
            catch (UserAgreementServiceException userAgreementServiceException)
            {
                return InternalServerError(userAgreementServiceException);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
        [InvisibleApi]
        [HttpPut]
        public async ValueTask<ActionResult<UserAgreement>> PutUserAgreementAsync(UserAgreement userAgreement)
        {
            try
            {
                UserAgreement modifiedUserAgreement =
                    await this.userAgreementService.ModifyUserAgreementAsync(userAgreement);

                return Ok(modifiedUserAgreement);
            }
            catch (UserAgreementValidationException userAgreementValidationException)
                when (userAgreementValidationException.InnerException
                    is NotFoundUserAgreementException)
            {
                return NotFound(userAgreementValidationException.InnerException);
            }
            catch (UserAgreementValidationException userAgreementValidationException)
            {
                return BadRequest(userAgreementValidationException.InnerException);
            }
            catch (UserAgreementDependencyValidationException userAgreementDependencyValidationException)
                when (userAgreementDependencyValidationException.InnerException
                    is AlreadyExistsUserAgreementException)
            {
                return Conflict(userAgreementDependencyValidationException.InnerException);
            }
            catch (UserAgreementDependencyValidationException userAgreementDependencyValidationException)
            {
                return BadRequest(userAgreementDependencyValidationException.InnerException);
            }
            catch (UserAgreementDependencyException userAgreementDependencyException)
            {
                return InternalServerError(userAgreementDependencyException);
            }
            catch (UserAgreementServiceException userAgreementServiceException)
            {
                return InternalServerError(userAgreementServiceException);
            }
        }

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
        [InvisibleApi]
        [HttpDelete("{userAgreementId}")]
        public async ValueTask<ActionResult<UserAgreement>> DeleteUserAgreementByIdAsync(Guid userAgreementId)
        {
            try
            {
                UserAgreement deletedUserAgreement =
                    await this.userAgreementService.RemoveUserAgreementByIdAsync(userAgreementId);

                return Ok(deletedUserAgreement);
            }
            catch (UserAgreementValidationException userAgreementValidationException)
                when (userAgreementValidationException.InnerException
                    is NotFoundUserAgreementException)
            {
                return NotFound(userAgreementValidationException.InnerException);
            }
            catch (UserAgreementValidationException userAgreementValidationException)
            {
                return BadRequest(userAgreementValidationException.InnerException);
            }
            catch (UserAgreementDependencyValidationException userAgreementDependencyValidationException)
                when (userAgreementDependencyValidationException.InnerException is LockedUserAgreementException)
            {
                return Locked(userAgreementDependencyValidationException.InnerException);
            }
            catch (UserAgreementDependencyValidationException userAgreementDependencyValidationException)
            {
                return BadRequest(userAgreementDependencyValidationException.InnerException);
            }
            catch (UserAgreementDependencyException userAgreementDependencyException)
            {
                return InternalServerError(userAgreementDependencyException);
            }
            catch (UserAgreementServiceException userAgreementServiceException)
            {
                return InternalServerError(userAgreementServiceException);
            }
        }
    }
}

