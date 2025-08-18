using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TsrmWebApi.Security.Models.PresentationModels
{
    public class P_LoginUP
    {
        //public string userID { get; set; }
        //public string password { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public decimal ProjectID { get; set; }
    }


    public class P_OTP
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DeviceAddress { get; set; }
        public string OTP { get; set; }
    }
    

    public class UserReg
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string contactNo { get; set; }
        public decimal RoleID { get; set; }
    }


    public class P_PasswordReset
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }



    public class P_SendSMS
    {
        public string MobileNo { get; set; }
        public string StrMessege { get; set; }
    }
}
