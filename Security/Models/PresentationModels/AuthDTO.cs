using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TsrmWebApi.Security.Models.PresentationModels
{
    public class AuthDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public int expires_in { get; set; }
        public string ActionTime { get; set; }
        
        public IEnumerable<string> Errors { get; set; }
    }
}
