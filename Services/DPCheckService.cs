using Oracle.ManagedDataAccess.Client;
using System.Data;
using TsrmWebApi.IServices;
using TsrmWebApi.Models.DataModels;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Oracle.ManagedDataAccess.Types;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;


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

        public async Task<DataTable> GetVisitReportConditionAsync(DateTime from, DateTime to, decimal? zoneid, decimal? divisionid, decimal? regionid, decimal? teritoryid)
        {
            var dt = new DataTable();

            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync(); // async open

            using var cmd = new OracleCommand("PRC_GET_DISTRIBUTOR_POINT_VISIT_DATA_DYNAMIC", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("p_fromdate", OracleDbType.Date).Value = from;
            cmd.Parameters.Add("p_todate", OracleDbType.Date).Value = to;
            cmd.Parameters.Add("p_zoneId", OracleDbType.Int32).Value = zoneid.HasValue ? (object)zoneid.Value : DBNull.Value;
            cmd.Parameters.Add("p_divisionId", OracleDbType.Int32).Value = divisionid.HasValue ? (object)divisionid.Value : DBNull.Value;
            cmd.Parameters.Add("p_regionId", OracleDbType.Int32).Value = regionid.HasValue ? (object)regionid.Value : DBNull.Value;
            cmd.Parameters.Add("p_teritoryId", OracleDbType.Int32).Value = teritoryid.HasValue ? (object)teritoryid.Value : DBNull.Value;

            var cursor = cmd.Parameters.Add("p_cur", OracleDbType.RefCursor);
            cursor.Direction = ParameterDirection.Output;

            // Wrap the synchronous Fill in Task.Run to avoid blocking
            await Task.Run(() =>
            {
                using var da = new OracleDataAdapter(cmd);
                da.Fill(dt);
            });

            return dt;
        }

        //public async Task<DataTable> GetVisitReportCondition(DateTime from, DateTime to)
        //{
        //    var dt = new DataTable();

        //    using var conn = new OracleConnection(_connectionString);
        //    await conn.OpenAsync(); // async open

        //    using var cmd = new OracleCommand("PRC_GET_DISTRIBUTOR_POINT_VISIT_DATA_DYNAMIC_2", conn)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };

        //    cmd.Parameters.Add("p_fromdate", OracleDbType.Date).Value = from;
        //    cmd.Parameters.Add("p_todate", OracleDbType.Date).Value = to;


        //    var cursor = cmd.Parameters.Add("p_cur", OracleDbType.RefCursor);
        //    cursor.Direction = ParameterDirection.Output;

        //    // Wrap the synchronous Fill in Task.Run to avoid blocking
        //    await Task.Run(() =>
        //    {
        //        using var da = new OracleDataAdapter(cmd);
        //        da.Fill(dt);

        //    });

        //    return dt;
        //}

        //public async Task<DataTable> GetVisitReportCondition(DateTime from, DateTime to)
        //{
        //    // Step 1: Row-wise fetch
        //    var dt = new DataTable();

        //    using var conn = new OracleConnection(_connectionString);
        //    await conn.OpenAsync();

        //    using var cmd = new OracleCommand("PRC_GET_DISTRIBUTOR_POINT_VISIT_DATA_DYNAMIC_2", conn)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };

        //    cmd.Parameters.Add("p_fromdate", OracleDbType.Date).Value = from;
        //    cmd.Parameters.Add("p_todate", OracleDbType.Date).Value = to;

        //    var cursor = cmd.Parameters.Add("p_cur", OracleDbType.RefCursor);
        //    cursor.Direction = ParameterDirection.Output;

        //    using (var da = new OracleDataAdapter(cmd))
        //    {
        //        da.Fill(dt);
        //    }

        //    if (dt.Rows.Count == 0)
        //        return dt; // empty table

        //    // Step 2: Create pivoted DataTable
        //    var pivotedDt = new DataTable();
        //    pivotedDt.Columns.Add("VISIT_ID");
        //    pivotedDt.Columns.Add("VISIT_DATE");
        //    pivotedDt.Columns.Add("ENROLL");

        //    // Get distinct question names
        //    var questionNames = dt.AsEnumerable()
        //                          .Select(r => r.Field<string>("QUESTION_NAME"))
        //                          .Distinct();

        //    foreach (var q in questionNames)
        //        pivotedDt.Columns.Add(q);

        //    // Step 3: Pivot rows
        //    var grouped = dt.AsEnumerable()
        //                    .GroupBy(r => new
        //                    {
        //                        VisitId = Convert.ToDecimal(r["VISIT_ID"]),
        //                        VisitDate = r.Field<string>("VISIT_DATE"),
        //                        Enroll = Convert.ToDecimal(r["ENROLL"])
        //                    });

        //    foreach (var g in grouped)
        //    {
        //        var newRow = pivotedDt.NewRow();
        //        newRow["VISIT_ID"] = g.Key.VisitId;
        //        newRow["VISIT_DATE"] = g.Key.VisitDate;
        //        newRow["ENROLL"] = g.Key.Enroll;

        //        foreach (var r in g)
        //        {

        //            var question = r["QUESTION_NAME"]?.ToString().Trim();
        //            var answer = r["ANSW"] == DBNull.Value ? "" : r["ANSW"].ToString();
        //            newRow[question] = answer;
        //        }

        //        pivotedDt.Rows.Add(newRow);
        //    }

        //    return pivotedDt;
        //}

        public async Task<DataTable> GetVisitReportCondition(DateTime from, DateTime to)
        {
            var dt = new DataTable();

            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("PRC_GET_DISTRIBUTOR_POINT_VISIT_DATA_DYNAMIC_2", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("p_fromdate", OracleDbType.Date).Value = from;
            cmd.Parameters.Add("p_todate", OracleDbType.Date).Value = to;

            var cursor = cmd.Parameters.Add("p_cur", OracleDbType.RefCursor);
            cursor.Direction = ParameterDirection.Output;

            using (var da = new OracleDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
                return dt;

            var pivotedDt = new DataTable();
            pivotedDt.Columns.Add("VISIT_ID");
            pivotedDt.Columns.Add("VISIT_DATE");
            pivotedDt.Columns.Add("ENROLL");

            var questionNames = dt.AsEnumerable()
                                  .Select(r => Regex.Replace(r["QUESTION_NAME"]?.ToString() ?? "", @"\s+", " ").Trim())
                                  .Distinct();

            foreach (var q in questionNames)
                pivotedDt.Columns.Add(q);

            var grouped = dt.AsEnumerable()
                            .GroupBy(r => new
                            {
                                VisitId = Convert.ToDecimal(r["VISIT_ID"]),
                                VisitDate = DateTime.ParseExact(r["VISIT_DATE"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture),
                                Enroll = Convert.ToDecimal(r["ENROLL"])
                            });

            foreach (var g in grouped)
            {
                var newRow = pivotedDt.NewRow();
                newRow["VISIT_ID"] = g.Key.VisitId;
                newRow["VISIT_DATE"] = g.Key.VisitDate.ToString("dd-MM-yyyy");
                newRow["ENROLL"] = g.Key.Enroll;

                foreach (var r in g)
                {
                    var question = Regex.Replace(r["QUESTION_NAME"]?.ToString() ?? "", @"\s+", " ").Trim();
                    var answer = r["ANSW"] == DBNull.Value ? "" : r["ANSW"].ToString().Trim();

                    if (!string.IsNullOrEmpty(newRow[question]?.ToString()))
                        newRow[question] += ", " + answer;
                    else
                        newRow[question] = answer;
                }

                pivotedDt.Rows.Add(newRow);
            }

            return pivotedDt;
        }

    }
}
