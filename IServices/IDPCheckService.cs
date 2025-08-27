using System.Data;
using TsrmWebApi.Models.DataModels;

namespace TsrmWebApi.IServices
{
    public interface IDPCheckService
    {
        Task<DataTable> GetDataLoadDPCheck(int loginId, int typeId,decimal questionid,decimal selectedid);

        Task<string> SubmitSurvey(SurveyAnswerDto input);

        Task<List<QuestionResponseJson>> GetQuestionResponsesAsync(int loginId, int typeId, int questionId, int sectionId);

        Task InsertVisitAsync(int enroll, string detailsJson);

        Task<IEnumerable<dynamic>> GetVisitReportAsync(DateTime fromDate, DateTime toDate, decimal enroll, int typeId);

        Task<string> GetVisitJsonAsync(int enrollId, DateTime fromDate, DateTime toDate);

        Task <DataTable> GetVisitReportConditionAsync(DateTime from, DateTime to, decimal? zoneid, decimal? divisionid, decimal? regionid, decimal? teritoryid);

        Task<DataTable> GetVisitReportCondition(DateTime from, DateTime to);

    }
}
