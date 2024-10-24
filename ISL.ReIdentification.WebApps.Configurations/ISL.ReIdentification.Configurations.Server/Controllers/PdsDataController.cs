// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.PdsDatas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Configurations.Server.Controllers
{
    [Authorize(Roles = "Administrators")]
    [ApiController]
    [Route("api/[controller]")]
    public class PdsDataController : RESTFulController
    {
        private readonly IPdsDataService pdsDataService;

        public PdsDataController(IPdsDataService pdsDataService) =>
            this.pdsDataService = pdsDataService;

        [HttpGet]
        [EnableQuery(PageSize = 25)]
        public async ValueTask<ActionResult<IQueryable<PdsData>>> GetAsync()
        {
            try
            {
                IQueryable<PdsData> retrievedPdsDatas = await this.pdsDataService.RetrieveAllPdsDatasAsync();

                return Ok(retrievedPdsDatas);
            }
            catch (PdsDataDependencyException pdsDataDependencyException)
            {
                return InternalServerError(pdsDataDependencyException);
            }
            catch (PdsDataServiceException pdsDataServiceException)
            {
                return InternalServerError(pdsDataServiceException);
            }
        }

        [HttpGet("{pdsDataId}")]
        public async ValueTask<ActionResult<PdsData>> GetPdsDataByIdAsync(Guid pdsDataId)
        {
            try
            {
                PdsData retrievedPdsData = await this.pdsDataService.RetrievePdsDataByIdAsync(pdsDataId);

                return Ok(retrievedPdsData);
            }
            catch (PdsDataValidationException pdsDataValidationException)
                when (pdsDataValidationException.InnerException is NotFoundPdsDataException)
            {
                return NotFound(pdsDataValidationException.InnerException);
            }
            catch (PdsDataValidationException pdsDataValidationException)
            {
                return BadRequest(pdsDataValidationException.InnerException);
            }
            catch (PdsDataDependencyException pdsDataDependencyException)
            {
                return InternalServerError(pdsDataDependencyException);
            }
            catch (PdsDataServiceException pdsDataServiceException)
            {
                return InternalServerError(pdsDataServiceException);
            }
        }

        [HttpDelete("{pdsDataId}")]
        public async ValueTask<ActionResult<PdsData>> DeletePdsDataByIdAsync(Guid pdsDataId)
        {
            PdsData pdsData = await this.pdsDataService.RemovePdsDataByIdAsync(pdsDataId);

            return Ok(pdsData);
        }
    }
}
