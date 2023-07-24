using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain.CertificationResults;

using Sabio.Models.Domain.Venues;
using Sabio.Models.Requests.CertificationResults;
using Sabio.Models.Requests.Certifications;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Data.SqlClient;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/certification/results")]
    [ApiController]
    public class CertificationResultsApiController : BaseApiController
    {
        private ICertificationResultsService _service = null;
        private IAuthenticationService<int> _authService = null;

        public CertificationResultsApiController(ICertificationResultsService service
            ,ILogger<CertificationResultsApiController> logger
            , IAuthenticationService<int> authenticationService) : base(logger)
        {
            _service = service;
            _authService = authenticationService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create (CertificationResultAddRequest model)
        {
            ObjectResult result = null;
            int userId = _authService.GetCurrentUserId();
            try
            {
                int id = _service.Add(model, userId);
                ItemResponse<int> response = new ItemResponse<int> { Item = id };
                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }
        [HttpPut("{id:int}")]
        // ajust api controller and or SQL 
        public ActionResult<SuccessResponse> Update(CertificationResultUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            IUserAuthData user = _authService.GetCurrentUser();

            try
            {
                _service.Update(model, user.Id);

                response = new SuccessResponse();
            }
            catch (Exception exception)
            {
                code = 500;
                response = new ErrorResponse(exception.Message);
                base.Logger.LogError(exception.ToString());
            }
            return StatusCode(code, response);
        }
        [HttpDelete]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;
                int userId = _authService.GetCurrentUserId();

            try
            {
                _service.Delete(id,userId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }
        [HttpGet("search")]
        public ActionResult<ItemResponse<Paged<CertificationResult>>> Search(int pageIndex, int pageSize, string query)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<CertificationResult> page = _service.Search(pageIndex, pageSize, query);
                if (page == null)
                {
                    code = 404;
                    response = new ItemResponse<Paged<CertificationResult>> { Item = page };
                }
                else
                {
                    response = new ItemResponse<Paged<CertificationResult>> { Item = page };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());

            }
            return StatusCode(code, response);
            }

            [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Paged<CertificationResult>>> Get(int id, int pageIndex, int pageSize)
        {
            int iCode = 200;
            BaseResponse response = null;
            try
            {
                Paged<CertificationResult> certificationResult = _service.Get(id,pageIndex,pageSize);

                if (certificationResult == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Address not found");
                }

                else
                {
                    response = new ItemResponse<Paged<CertificationResult>> { Item = certificationResult };

                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(iCode, response);

        }

    }

    
}
