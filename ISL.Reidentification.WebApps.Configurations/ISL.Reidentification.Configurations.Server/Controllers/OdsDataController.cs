// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.OdsDatas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Configurations.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OdsDataController : RESTFulController
    {
        private readonly IOdsDataService odsDataService;

        public OdsDataController(IOdsDataService odsDataService) =>
            this.odsDataService = odsDataService;

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
