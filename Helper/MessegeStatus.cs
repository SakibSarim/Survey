using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TsrmWebApi.Security.Models.PresentationModels;

namespace TsrmWebApi.Helper
{
    public class MessegeStatus
    {
        public object Data { get; set; }
        public object info { get; set; }
        public P_EmployeeDetails Info { get; internal set; }
        public Boolean Status { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
