// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.Lookups;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LookupsController : RESTFulController
    {
        private readonly ILookupService lookupService;

        public LookupsController(ILookupService lookupService) =>
            this.lookupService = lookupService;

        [HttpPost]
        public async ValueTask<ActionResult<Lookup>> PostLookupAsync(Lookup lookup)
        {
            try
            {
                Lookup addedLookup =
                    await this.lookupService.AddLookupAsync(lookup);

                return Created(addedLookup);
            }
            catch (LookupValidationException lookupValidationException)
            {
                return BadRequest(lookupValidationException.InnerException);
            }
            catch (LookupDependencyValidationException lookupValidationException)
                when (lookupValidationException.InnerException is InvalidLookupReferenceException)
            {
                return FailedDependency(lookupValidationException.InnerException);
            }
            catch (LookupDependencyValidationException lookupDependencyValidationException)
               when (lookupDependencyValidationException.InnerException is AlreadyExistsLookupException)
            {
                return Conflict(lookupDependencyValidationException.InnerException);
            }
            catch (LookupDependencyException lookupDependencyException)
            {
                return InternalServerError(lookupDependencyException);
            }
            catch (LookupServiceException lookupServiceException)
            {
                return InternalServerError(lookupServiceException);
            }
        }

        [HttpGet]
        public async ValueTask<ActionResult<IQueryable<Lookup>>> Get()
        {
            try
            {
                IQueryable<Lookup> retrievedLookups =
                    await this.lookupService.RetrieveAllLookupsAsync();

                return Ok(retrievedLookups);
            }
            catch (LookupDependencyException lookupDependencyException)
            {
                return InternalServerError(lookupDependencyException);
            }
            catch (LookupServiceException lookupServiceException)
            {
                return InternalServerError(lookupServiceException);
            }
        }

        [HttpGet("{lookupId}")]
        public async ValueTask<ActionResult<Lookup>> GetLookupByIdAsync(Guid lookupId)
        {
            try
            {
                Lookup lookup = await this.lookupService.RetrieveLookupByIdAsync(lookupId);

                return Ok(lookup);
            }
            catch (LookupValidationException lookupValidationException)
                when (lookupValidationException.InnerException is NotFoundLookupException)
            {
                return NotFound(lookupValidationException.InnerException);
            }
            catch (LookupValidationException lookupValidationException)
            {
                return BadRequest(lookupValidationException.InnerException);
            }
            catch (LookupDependencyException lookupDependencyException)
            {
                return InternalServerError(lookupDependencyException);
            }
            catch (LookupServiceException lookupServiceException)
            {
                return InternalServerError(lookupServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Lookup>> PutLookupAsync(Lookup lookup)
        {
            try
            {
                Lookup modifiedLookup =
                    await this.lookupService.ModifyLookupAsync(lookup);

                return Ok(modifiedLookup);
            }
            catch (LookupValidationException lookupValidationException)
                when (lookupValidationException.InnerException is NotFoundLookupException)
            {
                return NotFound(lookupValidationException.InnerException);
            }
            catch (LookupValidationException lookupValidationException)
            {
                return BadRequest(lookupValidationException.InnerException);
            }
            catch (LookupDependencyValidationException lookupValidationException)
                when (lookupValidationException.InnerException is InvalidLookupReferenceException)
            {
                return FailedDependency(lookupValidationException.InnerException);
            }
            catch (LookupDependencyValidationException lookupDependencyValidationException)
               when (lookupDependencyValidationException.InnerException is AlreadyExistsLookupException)
            {
                return Conflict(lookupDependencyValidationException.InnerException);
            }
            catch (LookupDependencyException lookupDependencyException)
            {
                return InternalServerError(lookupDependencyException);
            }
            catch (LookupServiceException lookupServiceException)
            {
                return InternalServerError(lookupServiceException);
            }
        }

        [HttpDelete("{lookupId}")]
        public async ValueTask<ActionResult<Lookup>> DeleteLookupByIdAsync(Guid lookupId)
        {
            try
            {
                Lookup deletedLookup =
                    await this.lookupService.RemoveLookupByIdAsync(lookupId);

                return Ok(deletedLookup);
            }
            catch (LookupValidationException lookupValidationException)
                when (lookupValidationException.InnerException is NotFoundLookupException)
            {
                return NotFound(lookupValidationException.InnerException);
            }
            catch (LookupValidationException lookupValidationException)
            {
                return BadRequest(lookupValidationException.InnerException);
            }
            catch (LookupDependencyValidationException lookupDependencyValidationException)
                when (lookupDependencyValidationException.InnerException is LockedLookupException)
            {
                return Locked(lookupDependencyValidationException.InnerException);
            }
            catch (LookupDependencyValidationException lookupDependencyValidationException)
            {
                return BadRequest(lookupDependencyValidationException);
            }
            catch (LookupDependencyException lookupDependencyException)
            {
                return InternalServerError(lookupDependencyException);
            }
            catch (LookupServiceException lookupServiceException)
            {
                return InternalServerError(lookupServiceException);
            }
        }
    }
}