using TsrmWebApi.Models.DataModels;

namespace TsrmWebApi.IServices
{
    public interface I_ConnectionResolver
    {
        public Task<ConnectionStringResult> GetConnectionString(decimal unitId);
    }
}
