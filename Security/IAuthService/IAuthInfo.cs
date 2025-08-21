using System.Threading.Tasks;
using TsrmWebApi.Models;
using TsrmWebApi.Security.Models.PresentationModels;

namespace TsrmWebApi.Security.IAuthService
{
    public interface IAuthInfo
    {
        Task<UserInfo> UserValidation(string UserName, string Password, decimal ProjectID);
        Task<P_EmployeeDetails> UserDetailsInfo(int UserEnroll, decimal projectid);
        public Task<P_EmployeeDetails> IUserInfobyUserEnroll(int UserEnroll);
        public Task<bool> ValidateRefreshToken(string refreshToken, string userName);
        public Task<TokenResponseModel> RefreshAccessToken(string refreshToken, string userName, string userId);

        Task<P_EmployeeDetails?> GetUserLoginFull(string userName, string password, decimal projectId);
    }
}
