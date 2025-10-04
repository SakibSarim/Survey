using TsrmWebApi.Models.DataModels;

namespace TsrmWebApi.IServices
{
    public interface IMarketVisitService
    {
        Task InsertSurveyVisitAsync(MarketVisitRequest model);

        Task<bool> InsertMarketVisitAsync(SurveyVisitRequest request);
    }
}
