using TsrmWebApi.Security.Models.PresentationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TsrmWebApi.Security.IAuthService
{
    public interface ITokenGenerator
    {
       
        public Task<AuthDTO> IidentityToken(string UserName, string UserID);

        public Task<TokenResponseModel> IidentityTokenRefresh(string userName, string userId);
        public Task<bool> ValidateRefreshToken(string refreshToken, string userName);
    }
}
