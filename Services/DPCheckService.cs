using Oracle.ManagedDataAccess.Client;
using System.Data;
using TsrmWebApi.IServices;
using TsrmWebApi.Models.DataModels;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Oracle.ManagedDataAccess.Types;
using System.Text;


namespace TsrmWebApi.Services
{
    public class DPCheckService : IDPCheckService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DPCheckService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("TSRM_Connection");
        }

        public async Task<DataTable> GetDataLoadDPCheck(int loginId, int typeId, decimal questionid, decimal selectedid)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = new OracleConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PRC_GET_DATA_LOAD_DP_CHECK";

                        // Input parameters
                        command.Parameters.Add("p_loginid", OracleDbType.Int32).Value = loginId;
                        command.Parameters.Add("p_typeid", OracleDbType.Int32).Value = typeId;
                        command.Parameters.Add("p_questionid", OracleDbType.Decimal).Value = questionid;
                        command.Parameters.Add("p_selectedid", OracleDbType.Decimal).Value = selectedid;


                        // Output parameter (only once!)
                        command.Parameters.Add("p_result_cur", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Error executing stored procedure: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error: " + ex.Message, ex);
            }

            return dt;
        }


        public async Task<string> SubmitSurvey(SurveyAnswerDto input)
        {
            using (var conn = new OracleConnection(_connectionString))
            {
                using (var cmd = new OracleCommand("PRC_DISTRIBUTOR_POINT_VISIT_INSERT", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_enroll", OracleDbType.Varchar2).Value = input.Enroll;
                    cmd.Parameters.Add("p_visit_date", OracleDbType.Date).Value = input.VisitDate;

                    cmd.Parameters.Add("p_status_list", OracleDbType.Object).UdtTypeName = "T_SURVEY_STATUS";
                    cmd.Parameters["p_status_list"].Value = input.StatusList.ToArray();

                    cmd.Parameters.Add("p_question_list", OracleDbType.Object).UdtTypeName = "T_QUESTION_ID";
                    cmd.Parameters["p_question_list"].Value = input.QuestionList.ToArray();

                    cmd.Parameters.Add("p_option_list", OracleDbType.Object).UdtTypeName = "T_SELECTED_OPTION";
                    cmd.Parameters["p_option_list"].Value = input.OptionList.ToArray();

                    cmd.Parameters.Add("p_attachment_list", OracleDbType.Object).UdtTypeName = "T_ATTACHMENT";
                    cmd.Parameters["p_attachment_list"].Value = input.AttachmentList.ToArray();

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return "Success";
        }

        public async Task<List<QuestionResponseJson>> GetQuestionResponsesAsync(int loginId, int typeId, int questionId, int sectionId)
        {
            var results = new List<QuestionResponseJson>();

            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("PRC_GET_DATA_LOAD_DP_CHECK_JASON", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_loginid", OracleDbType.Int32).Value = loginId;
            cmd.Parameters.Add("p_typeid", OracleDbType.Int32).Value = typeId;
            cmd.Parameters.Add("p_questionId", OracleDbType.Int32).Value = questionId;
            cmd.Parameters.Add("p_sectionnId", OracleDbType.Int32).Value = sectionId;

            var refCursorParam = new OracleParameter("p_result_cur", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.Parameters.Add(refCursorParam);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var questionIdVal = reader.GetInt32(reader.GetOrdinal("QUESTION_ID"));
                var responseJson = reader.GetString(reader.GetOrdinal("RESPONSE_JSON"));

                // Parse JSON string to List<ResponseItem>
                var responses = JsonSerializer.Deserialize<List<ResponseItem>>(responseJson);

                results.Add(new QuestionResponseJson
                {
                    QUESTION_ID = questionIdVal,
                    RESPONSES = responses
                });
            }

            return results;
        }

        public async Task InsertVisitAsync(int enroll, string detailsJson)
        {
            await using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "PRC_DISTRIBUTOR_POINT_QUESTION_VISIT_INSERT";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.Add("p_enroll", OracleDbType.Int32).Value = enroll;
            cmd.Parameters.Add("p_details_json", OracleDbType.Clob).Value = detailsJson;

            await cmd.ExecuteNonQueryAsync();
        }


        public async Task<IEnumerable<dynamic>> GetVisitReportAsync(DateTime fromDate, DateTime toDate, [FromQuery] decimal enroll, int typeId)
        {
            var result = new List<dynamic>();

            using (var conn = new OracleConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new OracleCommand("PRC_GET_DISTRIBUTOR_POINT_VISIT_QUESTION_REPORT", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("p_fromdate", OracleDbType.Date).Value = fromDate;
                    cmd.Parameters.Add("p_todate", OracleDbType.Date).Value = toDate;
                    cmd.Parameters.Add("p_enroll", OracleDbType.Decimal).Value = enroll;
                    cmd.Parameters.Add("p_typeid", OracleDbType.Int32).Value = typeId;

                    // Output cursor
                    cmd.Parameters.Add("p_result_cur", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            result.Add(row);
                        }
                    }
                }
            }

            return result;
        }

        public async Task<string> GetVisitJsonAsync(int enroll, DateTime fromDate, DateTime toDate)
        {
            using (var conn = new OracleConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new OracleCommand("PRC_GET_DISTRIBUTOR_POINT_VISIT_QUESTION_REPORT_JSON", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_enroll", OracleDbType.Int32).Value = enroll;
                    cmd.Parameters.Add("p_fromdate", OracleDbType.Date).Value = fromDate;
                    cmd.Parameters.Add("p_todate", OracleDbType.Date).Value = toDate;

                    // Output parameter for CLOB
                    var outputParam = new OracleParameter("p_result_json", OracleDbType.Clob)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    await cmd.ExecuteNonQueryAsync();

                    // Read the CLOB as string
                    if (outputParam.Value != DBNull.Value)
                    {
                        using (var clob = (OracleClob)outputParam.Value)
                        {
                            return clob.Value;
                        }
                    }

                    return "[]"; // return empty array if no data
                }
            }
        }



    }
}
