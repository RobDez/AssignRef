using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models.Domain.Locations;
using Sabio.Models.Domain;
using Sabio.Models.Requests.Games;
using Sabio.Services.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sabio.Models;
using Sabio.Models.Domain.Conferences;
using Sabio.Models.Domain.Teams;
using Sabio.Models.Domain.Seasons;
using Sabio.Models.Domain.TeamMembers;
using System.Reflection;
using Sabio.Models.Domain.Officials;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Sabio.Models.Domain.Games;
using System.Globalization;
using System.IO;
using System;
using System.Linq;
using System.Net;
using Sabio.Services.Locations;

namespace Sabio.Services
{
    public class GamesService : IGamesService, IMapGame
    {
        IDataProvider _data = null;
        ILookUpService _lookUpService = null;
        IMapBaseVenue _mapBaseVenue = null;
        ILocationMapper _locationMapper = null;
        IMapBaseConference _mapBaseConference = null;
        IMapBaseSeason _mapBaseSeason = null;
        IMapSingleTeam _mapSingleTeam = null;
        public GamesService(IDataProvider data, ILookUpService lookUpService, IMapBaseConference mapBaseConference, ILocationMapper locationMapper, IMapBaseVenue mapBaseVenue, IMapBaseSeason mapBaseSeason, IMapSingleTeam mapSingleTeam)
        {
            _data = data;
            _lookUpService = lookUpService;
            _mapBaseConference = mapBaseConference;
            _locationMapper = locationMapper;
            _mapBaseVenue = mapBaseVenue;
            _mapBaseSeason = mapBaseSeason;
            _mapSingleTeam = mapSingleTeam;
        }
        public Game GetById(int id)
        {
            string procName = "[dbo].[Games_Select_ById_V4]";
            Game game = null;
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIdex = 0;
                game = MapWithOfficials(reader, ref startingIdex);
            }
            );
            return game;
        }
        public List<Game> GetBySeasonId(int seasonId)
        {
            string procName = "[dbo].[Games_Select_BySeasonIdV2]";

            List<Game> list = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@SeasonId", seasonId);
                }
                , delegate (IDataReader reader, short set)
                {
                    int startingIdex = 0;
                    Game game = MapSingleGame(reader, ref startingIdex);

                    if (list == null)
                    {
                        list = new List<Game>();
                    }
                    list.Add(game);
                }
                );
            return list;
        }
        public Paged<Game> GetBySeasonIdPaginated(int pageIndex, int pageSize, int id)
        {
            Paged<Game> pagedList = null;
            List<Game> gameList = null;
            int totalCount = 0;

            string procName = "[dbo].[Games_Select_Paginated_BySeasonId_V3]";

            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@PageIndex", pageIndex);
                    collection.AddWithValue("@PageSize", pageSize);
                    collection.AddWithValue("@SeasonId", id);
                },
            singleRecordMapper: delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                Game aGame = MapSingleGame(reader, ref startingIndex);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }
                
                if (gameList == null)
                {
                    gameList = new List<Game>();
                }
                gameList.Add(aGame);
            });
            
            if (gameList != null)
            {
                pagedList = new Paged<Game>(gameList, pageIndex, pageSize, totalCount);

            }
            return pagedList;
        }

        public Paged<Game> GetBySeasonAndWeekPaginated(int pageIndex, int pageSize, int seasonId, int week)
        {
            Paged<Game> pagedList = null;
            List<Game> gameList = null;
            int totalCount = 0;

            string procName = "[dbo].[Games_Select_Paginated_ByWeek_SeasonIdV2]";

            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@PageIndex", pageIndex);
                    collection.AddWithValue("@PageSize", pageSize);
                    collection.AddWithValue("@SeasonId", seasonId);
                    collection.AddWithValue("@Week", week);
                },
            singleRecordMapper: delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                Game aGame = MapSingleGame(reader, ref startingIndex);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }

                if (gameList == null)
                {
                    gameList = new List<Game>();
                }
                gameList.Add(aGame);
            });

            if (gameList != null)
            {
                pagedList = new Paged<Game>(gameList, pageIndex, pageSize, totalCount);

            }
            return pagedList;
        }

        public Paged<Game> GetBySeasonWeekAndTeam(int pageIndex, int pageSize, int seasonId, int conferenceId, int? teamId = null, int? week = null )
        {
            Paged<Game> pagedList = null;
            List<Game> gameList = null;
            int totalCount = 0;

            string procName = "[dbo].[Games_Select_Paginated_ByWeek_SeasonId_Team]";

            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@ConferenceId", conferenceId);
                        collection.AddWithValue("@PageIndex", pageIndex);
                        collection.AddWithValue("@PageSize", pageSize);
                        collection.AddWithValue("@SeasonId", seasonId);
                        collection.AddWithValue("@Week", week);
                        collection.AddWithValue("@TeamId", teamId);
                   
                    
                    
                },
            singleRecordMapper: delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                Game aGame = MapSingleGame(reader, ref startingIndex);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }

                if (gameList == null)
                {
                    gameList = new List<Game>();
                }
                gameList.Add(aGame);
            });

            if (gameList != null)
            {
                pagedList = new Paged<Game>(gameList, pageIndex, pageSize, totalCount);

            }
            return pagedList;
        }

        public List<Game> GetBySeasonIdAndWeek(int seasonId, int week)
        {
            string procName = "[dbo].[Games_Select_ByWeek_SeasonIdV2]";

            List<Game> list = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@SeasonId", seasonId);
                parameterCollection.AddWithValue("@Week", week);
            }
                , delegate (IDataReader reader, short set)
                {
                    int startingIdex = 0;
                    Game game = MapSingleGame(reader, ref startingIdex);

                    if (list == null)
                    {
                        list = new List<Game>();
                    }
                    list.Add(game);
                }
                );
            return list;
        }
        public int Add(GameAddRequest model, int userId)
        {
            int id = 0;
            string procName = "[dbo].[Games_Insert]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col);
                col.AddWithValue("@CreatedBy", userId);


                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);

            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@Id"].Value;
                int.TryParse(oId.ToString(), out id);
            }
            );
            return id;
        }

        public List<BaseGameBulkInsert> BulkInsert(int conferenceId, int seasonId, IFormFile csvFile, int userId)
        {
            DataTable csvToUdt = null;
            string procName = "[dbo].[Games_InsertFromCsv]";

            List<BaseGameBulkInsert> records = new List<BaseGameBulkInsert>();

            using (var reader = new StreamReader(csvFile.OpenReadStream()))
            {
                var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    HeaderValidated = null,
                    BadDataFound = null,
                };
                using (CsvReader csvReader = new CsvReader(reader, configuration))
                {
                    records = csvReader.GetRecords<BaseGameBulkInsert>().ToList();
                }
            }
            csvToUdt = MapFileToTable(records);
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@CreatedBy", userId);
                col.AddWithValue("@ConferenceId", conferenceId);
                col.AddWithValue("@SeasonId", seasonId);
                col.AddWithValue("@Id", userId);
                col.AddWithValue("@newseason", csvToUdt);
            });
            return records;
        }
        private static DataTable MapFileToTable(List<BaseGameBulkInsert> records)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("StartTime", typeof(DateTime));
            dt.Columns.Add("Venue", typeof(string));
            dt.Columns.Add("HomeTeam", typeof(string));
            dt.Columns.Add("AwayTeam", typeof(string));
            dt.Columns.Add("Week", typeof(int));

            foreach (BaseGameBulkInsert record in records)
            {
                DataRow dr = dt.NewRow();
                int startingIndex = 0;

                dr.SetField(startingIndex++, record.StartTime);
                dr.SetField(startingIndex++, record.Venue);
                dr.SetField(startingIndex++, record.HomeTeam);
                dr.SetField(startingIndex++, record.AwayTeam);
                dr.SetField(startingIndex++, record.Week);
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public (List<BaseGameBulkInsert>, string) GetTablePreview(IFormFile csvFile)
        {
            bool isValid = false;
            string procName = "[dbo].[VerifyTeams]";
            string teamError = null;
            DataTable teamsTable = null;
            List<BaseGameBulkInsert> records = new List<BaseGameBulkInsert>();
            using (var reader = new StreamReader(csvFile.OpenReadStream()))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    HeaderValidated = null,
                    BadDataFound = null,

                };
                using (CsvReader csvReader = new CsvReader(reader, config))
                {
                    records = csvReader.GetRecords<BaseGameBulkInsert>().ToList();
                }

            }
            teamsTable = MapFileToPreviewTable(records);

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Teams", teamsTable);

                SqlParameter validation = new SqlParameter("@isValid", SqlDbType.Bit);
                validation.Direction = ParameterDirection.Output;
                col.Add(validation);

            }, returnParameters: delegate (SqlParameterCollection col)
            {
            isValid = (bool)col["@isValid"].Value;
            }
           );
            if (isValid == true)
            {
                return (records, null);
            }
            else
            {
                teamError = "Please check your team names";
                return (null, teamError);
            }
        }


        private static DataTable MapFileToPreviewTable(List<BaseGameBulkInsert> records)
        {
            DataTable dt = new DataTable();


            dt.Columns.Add("HomeTeam", typeof(string));
            dt.Columns.Add("AwayTeam", typeof(string));


            foreach (BaseGameBulkInsert record in records)
            {
                DataRow dr = dt.NewRow();
                int startingIndex = 0;


                dr.SetField(startingIndex++, record.HomeTeam);
                dr.SetField(startingIndex++, record.AwayTeam);

                dt.Rows.Add(dr);
            }
            // execute proc passing dt to it 
           // bool isValid

            return dt;
        }

        public void Update(GameUpdateRequest model, int userId)
        {
            string procName = "[dbo].[Games_Update]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col);
                col.AddWithValue("@Id", model.Id);
                col.AddWithValue("@ModifiedBy", userId);
            }, returnParameters: null
            );
        }
        public void Delete(int id, int userId)
        {
            string procName = "[dbo].[Games_Delete]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);
                col.AddWithValue("@ModifiedBy", userId);
            });
        }
        public Game MapSingleGame(IDataReader reader, ref int startingIdex)
        {
            Game aGame = new Game();
            aGame.Id = reader.GetSafeInt32(startingIdex++);
            aGame.GameStatus = _lookUpService.MapSingleLookUp(reader, ref startingIdex);
            aGame.Season = _mapBaseSeason.MapBaseSeason(reader, ref startingIdex);
            aGame.Week = reader.GetSafeInt32(startingIdex++);
            aGame.Conference = _mapBaseConference.MapBaseConference(reader, ref startingIdex);
            aGame.HomeTeam = _mapSingleTeam.MapSingleTeam(reader, ref startingIdex);
            aGame.VisitingTeam = _mapSingleTeam.MapSingleTeam(reader, ref startingIdex);
            aGame.StartTime = reader.GetSafeDateTime(startingIdex++);
            aGame.IsNonConference = reader.GetSafeBool(startingIdex++);
            aGame.Venue = _mapBaseVenue.MapBaseVenue(reader, ref startingIdex);
            aGame.DateCreated = reader.GetSafeDateTime(startingIdex++);
            aGame.DateModified = reader.GetSafeDateTime(startingIdex++);
            aGame.IsDeleted = reader.GetSafeBool(startingIdex++);
            aGame.CreatedBy = reader.GetSafeInt32(startingIdex++);
            aGame.ModifiedBy = reader.GetSafeInt32(startingIdex++);


            return aGame;
        }
        public Game MapWithOfficials(IDataReader reader, ref int startingIdex)
        {
            Game aGame = new Game();
            aGame.Id = reader.GetSafeInt32(startingIdex++);
            aGame.GameStatus = _lookUpService.MapSingleLookUp(reader, ref startingIdex);
            aGame.Season = _mapBaseSeason.MapBaseSeason(reader, ref startingIdex);
            aGame.Week = reader.GetSafeInt32(startingIdex++);
            aGame.Conference = _mapBaseConference.MapBaseConference(reader, ref startingIdex);
            aGame.HomeTeam = _mapSingleTeam.MapSingleTeam(reader, ref startingIdex);
            aGame.VisitingTeam = _mapSingleTeam.MapSingleTeam(reader, ref startingIdex);
            aGame.StartTime = reader.GetSafeDateTime(startingIdex++);
            aGame.IsNonConference = reader.GetSafeBool(startingIdex++);
            aGame.Venue = _mapBaseVenue.MapBaseVenue(reader, ref startingIdex);
            aGame.DateCreated = reader.GetSafeDateTime(startingIdex++);
            aGame.DateModified = reader.GetSafeDateTime(startingIdex++);
            aGame.IsDeleted = reader.GetSafeBool(startingIdex++);
            aGame.CreatedBy = reader.GetSafeInt32(startingIdex++);
            aGame.ModifiedBy = reader.GetSafeInt32(startingIdex++);
            aGame.Officials = reader.DeserializeObject<List<BaseOfficial>>(startingIdex++);

            return aGame;
        }
        private static void AddCommonParams(GameAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@StartTime", model.StartTime);
            col.AddWithValue("@VenueId", model.VenueId);
            col.AddWithValue("@SeasonId", model.SeasonId);
            col.AddWithValue("@Week", model.Week);
            col.AddWithValue("@HomeTeamId", model.HomeTeamId);
            col.AddWithValue("@VisitingTeamId", model.VisitingTeamId);
            col.AddWithValue("@isNonConference", model.IsNonConference);
            col.AddWithValue("@ConferenceId", model.ConferenceId);
        }
        public List<Game> GetBySeasonIdConferenceId(int seasonId, int conferenceId)
        {
            string procName = "[dbo].[Games_SelectBySeasonId_ConferenceIdV2]";

            List<Game> list = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@SeasonId", seasonId);
                parameterCollection.AddWithValue("@ConferenceId", conferenceId);
            }
                , delegate (IDataReader reader, short set)
                {
                    int startingIdex = 0;
                    Game game = MapWithOfficials(reader, ref startingIdex);

                    if (list == null)
                    {
                        list = new List<Game>();
                    }
                    list.Add(game);
                }
                );
            return list;
        }
    }
}
