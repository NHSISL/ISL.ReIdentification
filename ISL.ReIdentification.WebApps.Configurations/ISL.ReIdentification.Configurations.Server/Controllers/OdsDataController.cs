// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.OdsDatas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Configurations.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OdsDataController : RESTFulController
    {
        private readonly IOdsDataService odsDataService;

        public OdsDataController(IOdsDataService odsDataService) =>
            this.odsDataService = odsDataService;

        [HttpPost]
        public async ValueTask<ActionResult<OdsData>> PostOdsDataAsync([FromBody] OdsData odsData)
        {
            try
            {
                OdsData addedOdsData =
                    await this.odsDataService.AddOdsDataAsync(odsData);

                return Created(addedOdsData);
            }
            catch (OdsDataValidationException odsDataValidationException)
            {
                return BadRequest(odsDataValidationException.InnerException);
            }
            catch (OdsDataDependencyValidationException odsDataDependencyValidationException)
               when (odsDataDependencyValidationException.InnerException is AlreadyExistsOdsDataException)
            {
                return Conflict(odsDataDependencyValidationException.InnerException);
            }
            catch (OdsDataDependencyValidationException odsDataDependencyValidationException)
            {
                return BadRequest(odsDataDependencyValidationException.InnerException);
            }
            catch (OdsDataDependencyException odsDataDependencyException)
            {
                return InternalServerError(odsDataDependencyException);
            }
            catch (OdsDataServiceException odsDataServiceException)
            {
                return InternalServerError(odsDataServiceException);
            }
        }

        [HttpGet]
        [EnableQuery(PageSize = 25)]
        public async ValueTask<ActionResult<IQueryable<OdsData>>> Get()
        {
            try
            {
                IQueryable<OdsData> retrievedOdsDatas = await this.odsDataService.RetrieveAllOdsDatasAsync();

                return Ok(retrievedOdsDatas);
            }
            catch (OdsDataDependencyException odsDataDependencyException)
            {
                return InternalServerError(odsDataDependencyException);
            }
            catch (OdsDataServiceException odsDataServiceException)
            {
                return InternalServerError(odsDataServiceException);
            }
        }

        [HttpGet("GetChildren/{id}")]
        public async ValueTask<ActionResult<List<OdsData>>> GetAllChildren(Guid id)
        {
            try
            {
                List<OdsData> retrievedOdsDatas = await this.odsDataService.RetrieveChildrenByParentId(id);

                return Ok(retrievedOdsDatas);
            }
            catch (OdsDataDependencyException odsDataDependencyException)
            {
                return InternalServerError(odsDataDependencyException);
            }
            catch (OdsDataServiceException odsDataServiceException)
            {
                return InternalServerError(odsDataServiceException);
            }
        }

        [HttpGet("GetDescendants")]
        public async ValueTask<ActionResult<List<OdsData>>> GetAllDescendants(Guid id)
        {
            try
            {
                List<OdsData> retrievedOdsDatas = await this.odsDataService.RetrieveAllDecendentsByParentId(id);

                return Ok(retrievedOdsDatas);
            }
            catch (OdsDataDependencyException odsDataDependencyException)
            {
                return InternalServerError(odsDataDependencyException);
            }
            catch (OdsDataServiceException odsDataServiceException)
            {
                return InternalServerError(odsDataServiceException);
            }
        }

        [HttpGet("GetAncestors")]
        public async ValueTask<ActionResult<List<OdsData>>> GetAllAncestors(Guid id)
        {
            try
            {
                List<OdsData> retrievedOdsDatas = await this.odsDataService.RetrieveAllAncestorsByChildId(id);

                return Ok(retrievedOdsDatas);
            }
            catch (OdsDataDependencyException odsDataDependencyException)
            {
                return InternalServerError(odsDataDependencyException);
            }
            catch (OdsDataServiceException odsDataServiceException)
            {
                return InternalServerError(odsDataServiceException);
            }
        }

        [HttpGet("{odsDataId}")]
        public async ValueTask<ActionResult<OdsData>> GetOdsDataByIdAsync(Guid odsDataId)
        {
            try
            {
                OdsData retrievedOdsData = await this.odsDataService.RetrieveOdsDataByIdAsync(odsDataId);

                return Ok(retrievedOdsData);
            }
            catch (OdsDataValidationException odsDataValidationException)
                when (odsDataValidationException.InnerException is NotFoundOdsDataException)
            {
                return NotFound(odsDataValidationException.InnerException);
            }
            catch (OdsDataValidationException odsDataValidationException)
            {
                return BadRequest(odsDataValidationException.InnerException);
            }
            catch (OdsDataDependencyValidationException odsDataDependencyValidationException)
            {
                return BadRequest(odsDataDependencyValidationException.InnerException);
            }
            catch (OdsDataDependencyException odsDataDependencyException)
            {
                return InternalServerError(odsDataDependencyException);
            }
            catch (OdsDataServiceException odsDataServiceException)
            {
                return InternalServerError(odsDataServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<OdsData>> PutOdsDataAsync([FromBody] OdsData odsData)
        {
            try
            {
                OdsData modifiedOdsData =
                    await this.odsDataService.ModifyOdsDataAsync(odsData);

                return Ok(modifiedOdsData);
            }
            catch (OdsDataValidationException odsDataValidationException)
                when (odsDataValidationException.InnerException is NotFoundOdsDataException)
            {
                return NotFound(odsDataValidationException.InnerException);
            }
            catch (OdsDataValidationException odsDataValidationException)
            {
                return BadRequest(odsDataValidationException.InnerException);
            }
            catch (OdsDataDependencyValidationException odsDataDependencyValidationException)
               when (odsDataDependencyValidationException.InnerException is AlreadyExistsOdsDataException)
            {
                return Conflict(odsDataDependencyValidationException.InnerException);
            }
            catch (OdsDataDependencyValidationException odsDataDependencyValidationException)
            {
                return BadRequest(odsDataDependencyValidationException.InnerException);
            }
            catch (OdsDataDependencyException odsDataDependencyException)
            {
                return InternalServerError(odsDataDependencyException);
            }
            catch (OdsDataServiceException odsDataServiceException)
            {
                return InternalServerError(odsDataServiceException);
            }
        }

        [HttpDelete("{odsDataId}")]
        public async ValueTask<ActionResult<OdsData>> DeleteOdsDataByIdAsync(Guid odsDataId)
        {
            try
            {
                OdsData deletedOdsData =
                    await this.odsDataService.RemoveOdsDataByIdAsync(odsDataId);

                return Ok(deletedOdsData);
            }
            catch (OdsDataValidationException odsDataValidationException)
                when (odsDataValidationException.InnerException is NotFoundOdsDataException)
            {
                return NotFound(odsDataValidationException.InnerException);
            }
            catch (OdsDataValidationException odsDataValidationException)
            {
                return BadRequest(odsDataValidationException.InnerException);
            }
            catch (OdsDataDependencyValidationException odsDataDependencyValidationException)
                when (odsDataDependencyValidationException.InnerException is LockedOdsDataException)
            {
                return Locked(odsDataDependencyValidationException.InnerException);
            }
            catch (OdsDataDependencyValidationException odsDataDependencyValidationException)
            {
                return BadRequest(odsDataDependencyValidationException.InnerException);
            }
            catch (OdsDataDependencyException odsDataDependencyException)
            {
                return InternalServerError(odsDataDependencyException);
            }
            catch (OdsDataServiceException odsDataServiceException)
            {
                return InternalServerError(odsDataServiceException);
            }
        }
    }
}
