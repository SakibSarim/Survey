using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TsrmWebApi.Security.IAuthService;
using TsrmWebApi.Security.Models.PresentationModels;
using TsrmWebApi.Models;
using TsrmWebApi.Helper;
using Microsoft.AspNetCore.Authorization;
using TsrmWebApi.Models.PresentationModels;
using TsrmWebApi.Security.IAuthService;

namespace TsrmWebApi.Security.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInfo _authInfo;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthInfo authInfo, ITokenGenerator tokenGenerator, ILogger<AuthController> logger)
        {
            _authInfo = authInfo;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
        }

        [HttpPost]
        [Route("UserLogin")]
        public async Task<ActionResult<MessegeStatus>> UserLogin(P_LoginUP login)
        {
            var messageStatus = new MessegeStatus();

            try
            {
                string userName = login.UserName?.Trim();
                string password = login.Password?.Trim();
                decimal projectid = login.ProjectID;

                var userInfo = await _authInfo.UserValidation(userName, password, projectid);

                if (userInfo != null)
                {
                    var employeeInfo = await _authInfo.UserDetailsInfo(userInfo.UserId, projectid);

                    if (employeeInfo != null)
                    {
                        var token = await _tokenGenerator.IidentityToken(userName, employeeInfo.Employee_ID.ToString());

                        messageStatus = new MessegeStatus
                        {
                            Data = token,
                            Info = employeeInfo,
                            Status = true,
                            Code = 200,
                            Message = "Successfully Logged In"
                        };
                    }
                    else
                    {
                        messageStatus = new MessegeStatus
                        {
                            Data = Unauthorized(),
                            Status = false,
                            Code = 404,
                            Message = "Login Failed"
                        };
                    }
                }
                else
                {
                    messageStatus = new MessegeStatus
                    {
                        Data = Unauthorized(),
                        Status = false,
                        Code = 404,
                        Message = "Login Failed"
                    };
                }
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                messageStatus = new MessegeStatus
                {
                    Data = Unauthorized(),
                    Status = false,
                    Code = 500,
                    Message = "An error occurred during login"
                };
            }

            return Ok(messageStatus);
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<ActionResult<MessegeStatus>> RefreshToken([FromBody] TokenRequestModel tokenRequest)
        {
            var messageStatus = new MessegeStatus();

            try
            {
                bool isValid = await _tokenGenerator.ValidateRefreshToken(tokenRequest.RefreshToken, tokenRequest.UserName);

                if (!isValid)
                {
                    return Ok(new MessegeStatus
                    {
                        Data = null,
                        Status = false,
                        Code = 401,
                        Message = "Invalid refresh token"
                    });
                }

                // Validate and parse UserId
                if (!int.TryParse(tokenRequest.UserId, out int userId))
                {
                    return Ok(new MessegeStatus
                    {
                        Data = null,
                        Status = false,
                        Code = 400,
                        Message = "Invalid UserId format"
                    });
                }

                // Fetch employee info using parsed userId and provided projectid
                var employeeInfo = await _authInfo.UserDetailsInfo(userId, tokenRequest.projectid);

                if (employeeInfo == null)
                {
                    return Ok(new MessegeStatus
                    {
                        Data = null,
                        Status = false,
                        Code = 404,
                        Message = "User not found"
                    });
                }

                // Generate new token
                var newToken = await _tokenGenerator.IidentityToken(tokenRequest.UserName, tokenRequest.UserId);

                if (newToken != null && !string.IsNullOrEmpty(newToken.Token))
                {
                    Response.Cookies.Append("AuthToken", newToken.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.Now.AddSeconds(newToken.expires_in)
                    });
                }

                messageStatus = new MessegeStatus
                {
                    Data = newToken,
                    Info = employeeInfo,
                    Status = true,
                    Code = 200,
                    Message = "Token refreshed successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refresh token error for user: {User}", tokenRequest?.UserName);
                messageStatus = new MessegeStatus
                {
                    Data = null,
                    Status = false,
                    Code = 500,
                    Message = "An error occurred while refreshing token"
                };
            }

            return Ok(messageStatus);
        }

    }
}
