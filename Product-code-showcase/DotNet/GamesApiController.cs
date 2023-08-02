using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services;
using Web.Controllers;
using Web.Models.Responses;
using System;
using Models.Domain;
using System.Collections.Generic;
using Models.Requests.Games;
using Models;
using Web.Core.Services;
using Models.Domain.Games;
using Microsoft.AspNetCore.Http;

namespace Web.Api.Controllers
{
    [Route("api/games")]
    [ApiController]
    public class GamesApiController : BaseApiController
    {

        private IGamesService _gamesService = null;
        private IAuthenticationService<int> _authenticationService = null;
        public GamesApiController(IGamesService service
            , ILogger<GamesApiController> logger
            , IAuthenticationService<int> authenticationService) : base(logger)
        {
            _gamesService = service;
            _authenticationService = authenticationService;
        }
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Game>> GetById(int id)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                Game game = _gamesService.GetById(id);

                if (game == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Game> { Item = game };
                }
            }
            catch (Exception ex)
            {

                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }

            return StatusCode(iCode, response);

        }

        [HttpGet("season/{id:int}")]
        public ActionResult<ItemsResponse<Game>> GetBySeasonId(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<Game> list = _gamesService.GetBySeasonId(id);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemsResponse<Game> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }


            return StatusCode(code, response);

        }
        [HttpGet("season/pagination/{id:int}")]
        public ActionResult<ItemResponse<Paged<Game>>> GetBySeasonIdPaginated(int pageIndex, int pageSize, int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Game> game = _gamesService.GetBySeasonIdPaginated(pageIndex, pageSize, id);
                if (game == null)
                {
                    code = 404;
                    response = new ErrorResponse("search for paged list of files not found");
                }
                else
                {
                    response = new ItemResponse<Paged<Game>> { Item = game };
                }
            }
            catch (Exception ex)
            {

                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return (StatusCode(code, response));
        }

        [HttpGet("pagination/{id:int}")]
        public ActionResult<ItemResponse<Paged<Game>>> GetBySeasonAndWeekPaginated(int pageIndex, int pageSize, int week, int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Game> game = _gamesService.GetBySeasonAndWeekPaginated(pageIndex, pageSize, id, week);

                if (game == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Game>> { Item = game };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }


            return StatusCode(code, response);

        }

        [HttpGet("conference/{id:int}")]
        public ActionResult<ItemResponse<Paged<Game>>> GetBySeasonWeekAndTeam(int pageIndex, int pageSize, int id, int conferenceId, int? teamId = null, int? week = null )
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Game> game = _gamesService.GetBySeasonWeekAndTeam(pageIndex, pageSize, id, conferenceId, teamId, week);

                if (game == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Game>> { Item = game };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }


            return StatusCode(code, response);

        }



        [HttpGet("season/{id:int}/week/{week:int}")]
        public ActionResult<ItemsResponse<Game>> GetBySeasonIdAndWeek(int id, int week)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<Game> list = _gamesService.GetBySeasonIdAndWeek(id, week);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemsResponse<Game> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }


            return StatusCode(code, response);

        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(GameAddRequest model)
        {
            ObjectResult result = null;

            try
            {

                int userId = _authenticationService.GetCurrentUserId();
                int id = _gamesService.Add(model, userId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };

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

        [HttpPost("season/{conferenceId:int}/{seasonId:int}/batch")]
        public ActionResult<ItemResponse<BaseGameBulkInsert>> BulkInsert(int conferenceId, int seasonId, IFormFile csvFile)
        {
            int code = 200;
            BaseResponse response = null;
            IUserAuthData user = null;
            //int conferenceId = 0;
            try
            {
                user = _authenticationService.GetCurrentUser();

                
                //int.TryParse(user.TenantId.ToString(), out conferenceId);
                List<BaseGameBulkInsert> Season = _gamesService.BulkInsert(conferenceId, seasonId, csvFile, user.Id);


                response = new ItemsResponse<BaseGameBulkInsert> { Items = Season };

            }
            catch (Exception ex)
            {
                code = 500;
                Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);

            }

            return StatusCode(code, response);
        }

        [HttpPost("season")]
        public ActionResult<(ItemsResponse<BaseGameBulkInsert>, string)> GetTablePreview (IFormFile csvFile)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                (List<BaseGameBulkInsert> list, string teamError) = _gamesService.GetTablePreview(csvFile);
                if (list == null && teamError != null )
                {
                    code = 400;
                    response = new ErrorResponse("Team not found");
                }
                if (list == null && teamError == null)
                {
                    code = 400;
                    response = new ErrorResponse("Table not found.");
                }
                else if (list != null && teamError == null)
                {
                    response = new ItemsResponse<BaseGameBulkInsert> { Items = list };
                }

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }


        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(GameUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authenticationService.GetCurrentUserId();
                _gamesService.Update(model, userId);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authenticationService.GetCurrentUserId();
                _gamesService.Delete(id, userId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("conference/{conferenceId:int}/season/{seasonId:int}")]
        public ActionResult<ItemsResponse<Game>> GetBySeasonIdAndConferenceId(int seasonId, int conferenceId)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<Game> list = _gamesService.GetBySeasonIdConferenceId(seasonId, conferenceId);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemsResponse<Game> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }
    }
}
