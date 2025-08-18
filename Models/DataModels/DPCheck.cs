namespace TsrmWebApi.Models.DataModels
{
    public class DPCheck
    {
    }

    public class QuestionResponseJson
    {
        public int QUESTION_ID { get; set; }
        public List<ResponseItem> RESPONSES { get; set; }
    }

    public class ResponseItem
    {
        public string SELECTED_NAME { get; set; }
        public int SELECTED_OPTION { get; set; }
        public int? SELECTED_SUB_OPTION { get; set; }
        public string SELECTED_SUB_OPTION_NAME { get; set; }
    }

    public class SurveyAnswerDto
    {
        public string Enroll { get; set; }
        public DateTime VisitDate { get; set; }
        public List<int> StatusList { get; set; }
        public List<int> QuestionList { get; set; }
        public List<string> OptionList { get; set; } // Can be number or text
        public List<string> AttachmentList { get; set; }
    }


    public class DistributorVisitRequest
    {
        public string Enroll { get; set; }
        public DateTime VisitDate { get; set; }
        public List<QuestionAnswer> Answers { get; set; } = new();
    }

    public class QuestionAnswer
    {
        public int QuestionId { get; set; }
        public int? SelectedOption { get; set; } // Optional number
        public string SelectedOptionText { get; set; } // Optional text
        public string Attachment { get; set; }
    }

    public class DistributorVisitRequestCheck
    {
        public int Enroll { get; set; }
        public Dictionary<string, object> Details { get; set; }
    }

}
