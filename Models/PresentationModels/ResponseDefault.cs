using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
namespace TsrmWebApi.Models.PresentationModels
{
    public class ResponseDefault
    {
        public HttpStatusCode HttpStatusCode { set; get; }
        public string Msg { set; get; }
        public Boolean Success { set; get; }
        public DataTable data { get; set; }

    }
}