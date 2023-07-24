using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration.UserSecrets;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Games;
using Sabio.Models.Requests.Games;
using System.Collections.Generic;
using System.Data;

namespace Sabio.Services.Interfaces
{
    public interface IGamesService
    {
        int Add(GameAddRequest model, int userId);
        List<BaseGameBulkInsert> BulkInsert(int conferenceId, int seasonId, IFormFile csvFile, int userId);
        (List<BaseGameBulkInsert>, string) GetTablePreview(IFormFile csvFile);
        void Delete(int id, int userId);
        Game GetById(int id);
        List<Game> GetBySeasonId(int seasonId);
        Paged<Game> GetBySeasonIdPaginated(int pageIndex, int pageSize, int id);
        List<Game> GetBySeasonIdAndWeek(int seasonId, int week);
        void Update(GameUpdateRequest model, int userId);
        List<Game> GetBySeasonIdConferenceId(int seasonId, int conferenceId);
        public Paged<Game> GetBySeasonAndWeekPaginated(int pageIndex, int pageSize, int seasonId, int week);
        Paged<Game> GetBySeasonWeekAndTeam(int pageIndex, int pageSize, int seasonId, int conferenceId, int? team = null, int? week = null);
    }
}