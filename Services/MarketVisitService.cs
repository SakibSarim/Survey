using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text.Json;
using TsrmWebApi.IServices;
using TsrmWebApi.Models.DataModels;

namespace TsrmWebApi.Services
{
    public class MarketVisitService : IMarketVisitService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public MarketVisitService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("TSRM_Connection");
        }

        

        public async Task InsertSurveyVisitAsync(MarketVisitRequest model)
        {
            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("PRC_SURVEY_MARKET_VISIT_QUESTION_VISIT_INSERT", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Pass parameters
            cmd.Parameters.Add("p_enroll", OracleDbType.Int64).Value = model.Enroll;

            // Convert dictionary to JSON string
            var jsonString = System.Text.Json.JsonSerializer.Serialize(new { details = model.Details });
            cmd.Parameters.Add("p_details_json", OracleDbType.Clob).Value = jsonString;

            await cmd.ExecuteNonQueryAsync();
        }

     

        public async Task<bool> InsertMarketVisitAsync(SurveyVisitRequest request)
        {
            if (request == null || request.Details == null)
                throw new ArgumentNullException(nameof(request), "Request or Details cannot be null.");

            try
            {
                using var conn = new OracleConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new OracleCommand("PRC_SURVEY_MARKET_VISIT_QUESTION_VISIT_INSERT", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };


                cmd.Parameters.Add("p_enroll", OracleDbType.Int64).Value = request.Enroll;

                var formattedDict = new Dictionary<string, object>();

                foreach (var kvp in request.Details)
                {
                    if (kvp.Value is JToken token)
                    {
                        switch (token.Type)
                        {
                            case JTokenType.Integer:
                                formattedDict[kvp.Key] = token.Value<int>();
                                break;

                            case JTokenType.String:
                                formattedDict[kvp.Key] = token.Value<string>();
                                break;

                            case JTokenType.Array:
                               
                                var arr = token.Children<JValue>()
                                               .Where(x => x.Type == JTokenType.Integer)
                                               .Select(x => x.Value<int>())
                                               .ToArray();

                                if (arr.Length > 0)
                                    formattedDict[kvp.Key] = arr;
                                break;

                            default:
                               
                                break;
                        }
                    }
                    else
                    {
                        formattedDict[kvp.Key] = kvp.Value;
                    }
                }

                
                var jsonForOracle = Newtonsoft.Json.JsonConvert.SerializeObject(new { details = formattedDict });
                cmd.Parameters.Add("p_details_json", OracleDbType.Clob).Value = jsonForOracle;


                await cmd.ExecuteNonQueryAsync();

                return true;
            }
            catch
            {
                throw;
            }
        }





    }
}
