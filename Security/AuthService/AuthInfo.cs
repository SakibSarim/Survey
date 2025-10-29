using System;
using System.Data;
using System.Threading.Tasks;
using TsrmWebApi.Models;
using TsrmWebApi.Security.IAuthService;
using Oracle.ManagedDataAccess.Client;
using Microsoft.EntityFrameworkCore;
using TsrmWebApi.Models.DataModels;
using TsrmWebApi.Security.Models.PresentationModels;
using TsrmWebApi.Models.DataModels;
using System.Text.Json;

namespace TsrmWebApi.Security.AuthService
{
    public class AuthInfo : IAuthInfo
    {
        private readonly SurveyDbContext _ModelContext;
        private readonly ITokenGenerator _tokenGenerator;  // Declare the _tokenGenerator field

        public AuthInfo(SurveyDbContext modelContext, ITokenGenerator tokenGenerator)
        {
            _ModelContext = modelContext;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<UserInfo> UserValidation(string userName, string password, decimal ProjectID)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                throw new ArgumentNullException("UserName or Password cannot be null or empty");

            string connString = _ModelContext.Database.GetDbConnection().ConnectionString;

            try
            {
                using (var connection = new OracleConnection(connString))
                {
                    await connection.OpenAsync();

                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.CommandText = "PRC_Users_Login_Remote";

                        //cmd.Parameters.Add(new OracleParameter("userID", OracleDbType.Varchar2) { Value = userName });
                        //cmd.Parameters.Add(new OracleParameter("P_Password", OracleDbType.NVarchar2) { Value = password });

                        //var outParam = new OracleParameter("cur", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };

                        cmd.CommandText = "PRC_WEB_USERS_LOGIN";

                        cmd.Parameters.Add(new OracleParameter("p_empid", OracleDbType.Varchar2) { Value = userName });
                        cmd.Parameters.Add(new OracleParameter("p_password", OracleDbType.Varchar2) { Value = password });
                        cmd.Parameters.Add(new OracleParameter("p_projectid", OracleDbType.Decimal) { Value = ProjectID });

                        var outParam = new OracleParameter("p_cur", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(outParam);

                        using (var reader = (OracleDataReader)await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int userIdIndex = reader.GetOrdinal("login_id");
                                int fullNameIndex = reader.GetOrdinal("full_name");

                                return new UserInfo
                                {
                                    UserId = reader.IsDBNull(userIdIndex) ? 0 : Convert.ToInt32(reader.GetValue(userIdIndex)),
                                    UserName = reader.IsDBNull(fullNameIndex) ? string.Empty : reader.GetString(fullNameIndex)
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow or handle it as needed
                throw;
            }
        }

        public async Task<P_EmployeeDetails> IUserInfobyUserEnroll(int UserEnroll)
        {
            string connString = _ModelContext.Database.GetDbConnection().ConnectionString;

            try
            {
                using (var connection = new OracleConnection(connString))
                {
                    await connection.OpenAsync();

                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.CommandText = "PRC_Login_Emp_Info";

                        //cmd.Parameters.Add(new OracleParameter("UserEnroll", OracleDbType.Varchar2) { Value = UserEnroll.ToString() });

                        //var outParam = new OracleParameter("cur", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };


                        cmd.CommandText = "PRC_WEB_LOGIN_EMP_INFO";

                        cmd.Parameters.Add(new OracleParameter("p_empid", OracleDbType.Varchar2) { Value = UserEnroll.ToString() });
                        var outParam = new OracleParameter("p_cur", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(outParam);

                        using (var reader = (OracleDataReader)await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var roles = new List<int>();

                                // Read Users_Role as string and handle multiple roles
                                if (!reader.IsDBNull(reader.GetOrdinal("Users_Role")))
                                {
                                    string roleString = reader.GetString(reader.GetOrdinal("Users_Role"));
                                    var roleIds = roleString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var roleId in roleIds)
                                    {
                                        if (int.TryParse(roleId.Trim(), out int parsedRoleId))
                                        {
                                            roles.Add(parsedRoleId);
                                        }
                                        else
                                        {
                                            // Optional: log or handle conversion failure here
                                        }
                                    }
                                }
                                return new P_EmployeeDetails
                                {
                                    Employee_ID = reader.GetInt32(reader.GetOrdinal("emp_id")),
                                    Employee_Code = reader.GetString(reader.GetOrdinal("emp_no")),
                                    Employee_Name = reader.GetString(reader.GetOrdinal("full_name")),
                                    Unit_ID = reader.GetInt32(reader.GetOrdinal("comp_ID")),
                                    Users_Role = roles // Store the list of roles
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow or handle it as needed
                throw;
            }
        }
        public async Task<P_EmployeeDetails> UserDetailsInfo(int UserEnroll, decimal projectid)
        {
            string connString = _ModelContext.Database.GetDbConnection().ConnectionString;

            try
            {
                using (var connection = new OracleConnection(connString))
                {
                    await connection.OpenAsync();

                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.CommandText = "PRC_Login_Emp_Info";

                        //cmd.Parameters.Add(new OracleParameter("UserEnroll", OracleDbType.Varchar2) { Value = UserEnroll.ToString() });

                        //var outParam = new OracleParameter("cur", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };


                        cmd.CommandText = "PRC_WEB_LOGIN_EMP_INFO";

                        cmd.Parameters.Add(new OracleParameter("p_empid", OracleDbType.Varchar2) { Value = UserEnroll.ToString() });
                        cmd.Parameters.Add(new OracleParameter("p_projectid", OracleDbType.Decimal) { Value = projectid });
                        var outParam = new OracleParameter("p_cur", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(outParam);

                        using (var reader = (OracleDataReader)await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var roles = new List<int>();

                                // Read Users_Role as string and handle multiple roles
                                if (!reader.IsDBNull(reader.GetOrdinal("Users_Role")))
                                {
                                    string roleString = reader.GetString(reader.GetOrdinal("Users_Role"));
                                    var roleIds = roleString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var roleId in roleIds)
                                    {
                                        if (int.TryParse(roleId.Trim(), out int parsedRoleId))
                                        {
                                            roles.Add(parsedRoleId);
                                        }
                                        else
                                        {
                                            // Optional: log or handle conversion failure here
                                        }
                                    }
                                }
                                return new P_EmployeeDetails
                                {
                                    Employee_ID = reader.GetInt32(reader.GetOrdinal("emp_id")),
                                    Employee_Code = reader.GetString(reader.GetOrdinal("emp_no")),
                                    Employee_Name = reader.GetString(reader.GetOrdinal("full_name")),
                                    Unit_ID = reader.GetInt32(reader.GetOrdinal("comp_ID")),
                                    Users_Role = roles // Store the list of roles
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow or handle it as needed
                throw;
            }
        }

        public Task<bool> ValidateRefreshToken(string refreshToken, string userName)
        {
            return _tokenGenerator.ValidateRefreshToken(refreshToken, userName);
        }

        public async Task<TokenResponseModel> RefreshAccessToken(string refreshToken, string userName, string userId)
        {
            bool isValid = await ValidateRefreshToken(refreshToken, userName);
            if (!isValid)
                return null;

            return await _tokenGenerator.IidentityTokenRefresh(userName, userId);
        }

        public async Task<P_EmployeeDetails?> GetUserLoginFull(string userName, string password, decimal projectId)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return null;

            await using var connection = new OracleConnection(_ModelContext.Database.GetDbConnection().ConnectionString);
            await connection.OpenAsync();

            await using var command = new OracleCommand("PRC_WEB_USER_LOGIN_FULL", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("p_empid", OracleDbType.Varchar2).Value = userName;
            command.Parameters.Add("p_password", OracleDbType.Varchar2).Value = password;
            command.Parameters.Add("p_projectid", OracleDbType.Decimal).Value = projectId;
            command.Parameters.Add("p_cur", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (await reader.ReadAsync())
            {
                var roles = new List<int>();
                if (!reader.IsDBNull(reader.GetOrdinal("Users_Role")))
                {
                    string roleString = reader.GetString(reader.GetOrdinal("Users_Role"));
                    roles = roleString
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(r => int.TryParse(r.Trim(), out int parsed) ? parsed : 0)
                        .Where(r => r != 0)
                        .ToList();
                }

                return new P_EmployeeDetails
                {
                    Employee_ID = reader.GetInt32(reader.GetOrdinal("emp_id")),
                    Employee_Code = reader.GetString(reader.GetOrdinal("emp_no")),
                    Employee_Name = reader.GetString(reader.GetOrdinal("full_name")),
                    Unit_ID = reader.GetInt32(reader.GetOrdinal("comp_ID")),
                    Users_Role = roles
                };
            }

            return null;
        }

        public async Task<UserDetailsFinal?> GetUserLoginFullHierarchy(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return null;

            UserDetailsFinal? user = null;

            await using var connection = new OracleConnection(_ModelContext.Database.GetDbConnection().ConnectionString);
            await connection.OpenAsync();

            await using var cmd = new OracleCommand("PRC_WEB_USER_LOGIN_FULL_HIERARCHY_OPT", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("p_empid", OracleDbType.Varchar2).Value = userName;
            cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = password;
            cmd.Parameters.Add("p_cur", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                string? usersRoleJson = reader["Users_Role"]?.ToString();

                user = new UserDetailsFinal
                {
                    Employee_ID = reader["emp_id"] != DBNull.Value ? Convert.ToInt32(reader["emp_id"]) : 0,
                    Employee_Code = reader["emp_no"]?.ToString() ?? string.Empty,
                    Employee_Name = reader["full_name"]?.ToString() ?? string.Empty,
                    Unit_ID = reader["comp_id"] != DBNull.Value ? Convert.ToInt32(reader["comp_id"]) : 0,
                    Users_Role = string.IsNullOrEmpty(usersRoleJson)
                        ? new List<ProjectRoleFinal>()
                        : JsonSerializer.Deserialize<List<ProjectRoleFinal>>(usersRoleJson)
                };
            }

            return user;
        }
    }
}
