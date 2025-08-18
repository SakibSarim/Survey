using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TsrmWebApi.Security.Models.PresentationModels
{
    public class P_EmployeeDetails
    {
        public int Employee_ID { get; set; }
        public string Employee_Code { get; set; }
        public string Employee_Name { get; set; }
        public int Unit_ID { get; set; }
        public List<int> Users_Role { get; set; } // Changed to a list
    }
}
