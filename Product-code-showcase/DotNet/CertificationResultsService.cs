using Data.Providers;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

using Text;
using System.Threading.Tasks;
using Models.Requests.CertificationResults;
using Models.Domain;
using Models.Domain.CertificationResults;
using Data;
using Models.Domain.Certifications;
using Models.Domain.Seasons;
using Models.Domain.Tests;
using Models;
using Models.Domain.Venues;
using Models.Requests.Certifications;

namespace Services
{
    public class CertificationResultsService : ICertificationResultsService
    {
        IDataProvider _data = null;
        IBaseUserMapper _baseUserMapper = null;

        public CertificationResultsService(IDataProvider data, IBaseUserMapper userMapper)
        {
            _data = data;
            _baseUserMapper = userMapper;
        }

        public int Add(CertificationResultAddRequest model, int userId)
        {
            int Id = 0;
            string procName = "[dbo].[CertificationResults_Insert]";

            _data.ExecuteNonQuery(
                procName,
                inputParamMapper: delegate(SqlParameterCollection col)
                {
                    AddCertificateResultParams(model, col);
                    col.AddWithValue("@CreatedBy", userId);

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;

                    col.Add(idOut);
                },
                returnParameters: delegate(SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;
                    int.TryParse(returnCollection["@Id"].Value.ToString(), out Id);
                }
            );
            return Id;
        }

        public void Update(CertificationResultUpdateRequest model, int userId)
        {
            string procedureName = "CertificationResults_Update";

            _data.ExecuteNonQuery(
                procedureName,
                inputParamMapper: delegate(SqlParameterCollection collection)
                {
                    AddCertificateResultParams(model, collection);

                    collection.AddWithValue("@ModifiedBy", userId);
                    collection.AddWithValue("@Id", model.Id);
                },
                returnParameters: null
            );
        }

        public void Delete(int id, int userId)
        {
            string procName = "[dbo].[CertificationResults_Delete]";
            _data.ExecuteNonQuery(
                procName,
                inputParamMapper: delegate(SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", id);
                    col.AddWithValue("@ModifiedBy", userId);
                },
                returnParameters: null
            );
        }

        public Paged<CertificationResult> Search(int pageIndex, int pageSize, string query)
        {
            Paged<CertificationResult> pagedList = null;
            List<CertificationResult> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(
                "[dbo].[CertificationResults_Search]",
                (SqlParameterCollection col) =>
                {
                    col.AddWithValue("@PageIndex", pageIndex);
                    col.AddWithValue("@PageSize", pageSize);
                    col.AddWithValue("@Query", query);
                },
                (reader, readerSetIndex) =>
                {
                    int startingIndex = 0;
                    CertificationResult certificationResult = MapCertificationResult(
                        reader,
                        ref startingIndex
                    );

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startingIndex);
                    }
                    if (list == null)
                    {
                        list = new List<CertificationResult>();
                    }
                    list.Add(certificationResult);
                }
            );
            if (list != null)
            {
                pagedList = new Paged<CertificationResult>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public Paged<CertificationResult> Get(int id, int pageIndex, int pageSize)
        {
            Paged<CertificationResult> pagedResults = null;

            List<CertificationResult> listResult = null;

            int totalCount = 0;
            string procName = "[dbo].[CertificationResults_SelectByCertificationId]";
            _data.ExecuteCmd(
                procName,
                delegate(SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@CertificationId", id);
                    paramCollection.AddWithValue("@PageIndex", pageIndex);
                    paramCollection.AddWithValue("@PageSize", pageSize);
                },
                delegate(IDataReader reader, short set)
                {
                    CertificationResult certResult = new CertificationResult();
                    int startingIndex = 0;
                    certResult = MapCertificationResult(reader, ref startingIndex);
                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startingIndex++);
                    }
                    if (listResult == null)
                    {
                        listResult = new List<CertificationResult>();
                    }
                    listResult.Add(certResult);
                }
            );
            if (listResult != null)
            {
                pagedResults = new Paged<CertificationResult>(
                    listResult,
                    pageIndex,
                    pageSize,
                    totalCount
                );
            }
            return pagedResults;
        }

        private CertificationResult MapCertificationResult(
            IDataReader reader,
            ref int startingIndex
        )
        {
            CertificationResult singleCertResult = new CertificationResult();

            singleCertResult.Certification = new Certification();
            singleCertResult.Certification.Season = new Season();
            singleCertResult.Certification.Test = new TestResultBase();

            singleCertResult.Id = reader.GetSafeInt32(startingIndex++);

            singleCertResult.Certification.Id = reader.GetSafeInt32(startingIndex++);
            singleCertResult.Certification.Name = reader.GetSafeString(startingIndex++);

            singleCertResult.Certification.Season.Id = reader.GetSafeInt32(startingIndex++);
            singleCertResult.Certification.Season.Name = reader.GetSafeString(startingIndex++);
            singleCertResult.Certification.Season.Year = reader.GetSafeInt32(startingIndex++);

            singleCertResult.Certification.IsPhysicalRequired = reader.GetSafeBool(startingIndex++);
            singleCertResult.Certification.IsBackgroundCheckRequired = reader.GetSafeBool(
                startingIndex++
            );
            singleCertResult.Certification.IsTestRequired = reader.GetSafeBool(startingIndex++);

            singleCertResult.Certification.Test.Id = reader.GetSafeInt32(startingIndex++);
            singleCertResult.Certification.Test.Name = reader.GetSafeString(startingIndex++);
            singleCertResult.Certification.Test.MinimumScoreRequired =
                reader.GetSafeDecimalNullable(startingIndex++);
            singleCertResult.Certification.IsFitnessTestRequired = reader.GetSafeBool(
                startingIndex++
            );
            singleCertResult.Certification.IsClinicRequired = reader.GetSafeBool(startingIndex++);
            singleCertResult.Certification.DueDate = reader.GetDateTime(startingIndex++);
            singleCertResult.Certification.IsActive = reader.GetSafeBool(startingIndex++);

            singleCertResult.Certification.CreatedBy = _baseUserMapper.MapBaseUser(
                reader,
                ref startingIndex
            );
            singleCertResult.Certification.ModifiedBy = _baseUserMapper.MapBaseUser(
                reader,
                ref startingIndex
            );

            singleCertResult.IsPhysicalCompleted = reader.GetSafeBool(startingIndex++);
            singleCertResult.IsBackgroundCheckCompleted = reader.GetSafeBool(startingIndex++);
            singleCertResult.IsTestCompleted = reader.GetSafeBool(startingIndex++);
            singleCertResult.TestInstanceId = reader.GetSafeInt32(startingIndex++);
            singleCertResult.Score = reader.GetSafeDecimal(startingIndex++);
            singleCertResult.IsFitnessTestCompleted = reader.GetSafeBool(startingIndex++);
            singleCertResult.IsClinicAttended = reader.GetSafeBool(startingIndex++);
            singleCertResult.IsCompleted = reader.GetSafeBool(startingIndex++);
            singleCertResult.IsActive = reader.GetSafeBool(startingIndex++);

            singleCertResult.User = _baseUserMapper.MapBaseUser(reader, ref startingIndex);

            singleCertResult.CreatedBy = reader.GetSafeInt32(startingIndex++);
            singleCertResult.ModifiedBy = reader.GetSafeInt32(startingIndex++);

            singleCertResult.DateCreated = reader.GetSafeDateTime(startingIndex++);
            singleCertResult.DateModified = reader.GetSafeDateTime(startingIndex++);

            return singleCertResult;
        }

        private static void AddCertificateResultParams(
            CertificationResultAddRequest model,
            SqlParameterCollection col
        )
        {
            col.AddWithValue("@CertificationId", model.CertificationId);
            col.AddWithValue("@IsPhysicalCompleted", model.IsPhysicalCompleted);
            col.AddWithValue("@IsBackgroundCheckCompleted", model.IsBackgroundCheckCompleted);
            col.AddWithValue("@IsTestCompleted", model.IsTestCompleted);
            col.AddWithValue("@TestInstanceId", model.TestInstanceId);
            col.AddWithValue("@Score", model.Score);
            col.AddWithValue("@IsFitnessTestCompleted", model.IsFitnessTestCompleted);
            col.AddWithValue("@IsClinicAttended", model.IsClinicAttended);
            col.AddWithValue("@IsActive", model.IsActive);
            col.AddWithValue("@UserId", model.UserId);
        }
    }
}
