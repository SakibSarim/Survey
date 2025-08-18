using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TsrmWebApi.Models.PresentationModels
{
    public class ItemDetail
    {

        public int CompId { get; set; }
        public string ItemParentCode { get; set; }
       // public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public string CreateBy { get; set; }
       // public DateTime CreateDate { get; set; }
        //public int UnitId { get; set; }
        public string Status { get; set; }

    }

    public class ItemDto
    {
        public string ItemParentCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
    }
}
