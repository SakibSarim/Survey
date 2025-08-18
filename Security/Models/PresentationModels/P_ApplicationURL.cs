using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TsrmWebApi.Security.Models.PresentationModels
{
    public class P_ApplicationURL
    {
        public string MotherMenu { get; set; }
        public List<P_ApplicationURLChild> p_ApplicationURLChildren { get; set; }
    }

    public class P_ApplicationURLChild
    {
        public string path { get; set; }
        public string name { get; set; }
        public string component { get; set; }
        public string Displayname { get; set; }
        public string Icon { get; set; }
        public decimal menuSl { get; set; }
    }


    public class P_ApplicationURLMOTHERANCHILD
    {
        public decimal MotherMenuID { get; set; }
        public string MotherMenuName { get; set; }
        public decimal CHILDMENUID { get; set; }
        public decimal menuSl { get; set; }
        public string path { get; set; }
        public string name { get; set; }
        public string component { get; set; }
        public string Displayname { get; set; }
        public string ICON { get; set; }
    }
}
