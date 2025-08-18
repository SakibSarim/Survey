using TsrmWebApi.IServices;
using TsrmWebApi.Models.DataModels;

namespace TsrmWebApi.Services
{
    public class S_ConnectionResolver : I_ConnectionResolver
    {
        private readonly IConfiguration _configuration;

        public S_ConnectionResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ConnectionStringResult> GetConnectionString(decimal unitId)
        {
            return await Task.Run(() =>
            {
                var connectionStrings = new List<string>();
                string localDirectoryPath = string.Empty;

                if (unitId == 1)
                {
                    connectionStrings.Add(_configuration.GetConnectionString("ITAsset_Connection"));
                    localDirectoryPath = @"E:\IT_ASSET\All_Units_IT_ASSET"; // Default for unit 1
                }
                else
                {
                    string singleConnectionString = unitId switch
                    {
                        53 => _configuration.GetConnectionString("ITAsset_Connection"),
                        98 => _configuration.GetConnectionString("ITAsset_Connection"),
                        14 => _configuration.GetConnectionString("ITAsset_Connection"),
                        300 => _configuration.GetConnectionString("ITAsset_Connection"),

                        _ => null
                    };

                    localDirectoryPath = unitId switch
                    {
                        //Local Path
                        53 => @"E:\IT_ASSET\AFML_IT_ASSET_DOC",
                        98 => @"E:\IT_ASSET\ABL_HRMS_IT_ASSET_DOC",
                        14 => @"E:\IT_ASSET\APPL_HRMS_IT_ASSET_DOC",
                        300 => @"E:\IT_ASSET\ATML_HRMS_IT_ASSET_DOC",

                        _ => @"E:\IT_ASSET\Default_FILE" // Default path if no match
                    };

                    if (singleConnectionString != null)
                    {
                        connectionStrings.Add(singleConnectionString);
                    }
                }

                if (!connectionStrings.Any())
                {
                    return new ConnectionStringResult
                    {
                        Success = false,
                        Message = "Invalid Unit ID. No connection string found."
                    };
                }

                return new ConnectionStringResult
                {
                    Success = true,
                    ConnectionStrings = connectionStrings,
                    LocalDirectoryPath = localDirectoryPath
                };
            });
        }
    }
}
