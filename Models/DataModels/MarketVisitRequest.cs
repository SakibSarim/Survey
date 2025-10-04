using System.Text.Json;

namespace TsrmWebApi.Models.DataModels
{
    public class MarketVisitRequest
    {
        public long Enroll { get; set; }
        public Dictionary<int, object> Details { get; set; } = new();
    }

     public class SurveyVisitRequest
    {
        public long Enroll { get; set; }

        public Dictionary<string, object> Details { get; set; }
    }
}
