using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TsrmWebApi.Security.Models.PresentationModels
{
    public class Audience
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audiance { get; set; }
    }
}
