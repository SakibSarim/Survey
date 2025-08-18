using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TsrmWebApi.Models.PresentationModels
{
    public class SMSApiModel
    {
        public string ContactNo { get; set; }
        public string EventTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public int intUnitID { get; set; }
        public bool ReturnValue { get; internal set; }
    }

    public class SMSApiModelUpdate
    {
        public required string  MeetingID { get; set; }
        public string ContactNo { get; set; }
        public string EventTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public int intUnitID { get; set; }
        public bool ReturnValue { get; internal set; }
    }
}
