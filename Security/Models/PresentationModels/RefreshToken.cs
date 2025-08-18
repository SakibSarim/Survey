namespace TsrmWebApi.Security.Models.PresentationModels
{
    public class RefreshToken
    {
    }
    public class TokenResponseModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    // TokenRequestModel.cs
    public class TokenRequestModel
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public decimal projectid { get; set; }
        public string RefreshToken { get; set; }
    }
}
