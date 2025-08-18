using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TsrmWebApi.Helper
{
    public class MessageStatus
    {
        public object Data { get; set; }
        public object info { get; set; }
        public Boolean Status { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
