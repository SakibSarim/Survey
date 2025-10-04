using System.Data;

namespace TsrmWebApi.IServices
{
    public interface IReportService
    {
        Task<DataTable> GetVisitReportCondition(DateTime from, DateTime to, int typeid);
    }
}
