using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using TsrmWebApi.IServices;

namespace TsrmWebApi.Services
{
    public class ReportService : IReportService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ReportService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("TSRM_Connection");
        }
        //public async Task<DataTable> GetVisitReportCondition(DateTime from, DateTime to, int typeid)
        //{
        //    var dt = new DataTable();

        //    using var conn = new OracleConnection(_connectionString);
        //    await conn.OpenAsync();

        //    using var cmd = new OracleCommand("PRC_GET_SURVEY_CONDITIONAL_REPORT", conn)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };

        //    cmd.Parameters.Add("p_fromdate", OracleDbType.Date).Value = from;
        //    cmd.Parameters.Add("p_todate", OracleDbType.Date).Value = to;
        //    cmd.Parameters.Add("p_typeid", OracleDbType.Int32).Value = typeid;

        //    var cursor = cmd.Parameters.Add("p_cur", OracleDbType.RefCursor);
        //    cursor.Direction = ParameterDirection.Output;

        //    using (var da = new OracleDataAdapter(cmd))
        //    {
        //        da.Fill(dt);
        //    }

        //    if (dt.Rows.Count == 0)
        //        return dt;

        //    var pivotedDt = new DataTable();
        //    pivotedDt.Columns.Add("VISIT_ID");
        //    pivotedDt.Columns.Add("VISIT_DATE");
        //    pivotedDt.Columns.Add("ENROLL");

        //    var questionNames = dt.AsEnumerable()
        //                          .Select(r => Regex.Replace(r["QUESTION_NAME"]?.ToString() ?? "", @"\s+", " ").Trim())
        //                          .Distinct();

        //    foreach (var q in questionNames)
        //        pivotedDt.Columns.Add(q);

        //    var grouped = dt.AsEnumerable()
        //                    .GroupBy(r => new
        //                    {
        //                        VisitId = Convert.ToDecimal(r["VISIT_ID"]),
        //                        VisitDate = DateTime.ParseExact(r["VISIT_DATE"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture),
        //                        Enroll = Convert.ToDecimal(r["ENROLL"])
        //                    });

        //    foreach (var g in grouped)
        //    {
        //        var newRow = pivotedDt.NewRow();
        //        newRow["VISIT_ID"] = g.Key.VisitId;
        //        newRow["VISIT_DATE"] = g.Key.VisitDate.ToString("dd-MM-yyyy");
        //        newRow["ENROLL"] = g.Key.Enroll;

        //        foreach (var r in g)
        //        {
        //            var question = Regex.Replace(r["QUESTION_NAME"]?.ToString() ?? "", @"\s+", " ").Trim();
        //            var answer = r["ANSW"] == DBNull.Value ? "" : r["ANSW"].ToString().Trim();

        //            if (!string.IsNullOrEmpty(newRow[question]?.ToString()))
        //                newRow[question] += ", " + answer;
        //            else
        //                newRow[question] = answer;
        //        }

        //        pivotedDt.Rows.Add(newRow);
        //    }

        //    return pivotedDt;
        //}

        public async Task<DataTable> GetVisitReportCondition(DateTime from, DateTime to, int typeid)
        {
            var dt = new DataTable();

            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("PRC_GET_SURVEY_CONDITIONAL_REPORT", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("p_fromdate", OracleDbType.Date).Value = from;
            cmd.Parameters.Add("p_todate", OracleDbType.Date).Value = to;
            cmd.Parameters.Add("p_typeid", OracleDbType.Int32).Value = typeid;

            var cursor = cmd.Parameters.Add("p_cur", OracleDbType.RefCursor);
            cursor.Direction = ParameterDirection.Output;

            using (var da = new OracleDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            // No rows, return empty DataTable
            if (dt.Rows.Count == 0)
                return dt;

            // If typeid = 1 (non-pivoted), optionally pivot in C# or just return as is
            if (typeid == 1)
            {
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
            else // typeid = 2, pivoted from stored procedure
            {
                // The ref cursor already returns pivoted columns (one row per VISIT_ID)
                return dt;
            }
        }

    }
}
