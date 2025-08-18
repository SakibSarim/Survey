using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace TsrmWebApi.Models.PresentationModels
{
    public class UserInfo
    {
        //public int userID { get; set; }
        //public string password { get; set; }

          public int UserId { get; set; }
       // public string UserId { get; set; }
        public string UserName { get; set; }


    }
}
