using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TsrmWebApi.Security.Models.PresentationModels
{
    public class UserInfo
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string UserTytpe { get; set; }
        public string RoleID { get; set; }
        public string DeviceAddress { get; set; }
        public string Contract { get; set; }
    }
}
