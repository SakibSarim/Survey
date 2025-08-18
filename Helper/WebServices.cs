using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
namespace TsrmWebApi.Helper
{
    public class WebServices
    {
        public static string SendSMS(string Phoneno, string SMS)
        {
            try
            {
                string SSLURI = "https://smsplus.sslwireless.com/api/v3/send-sms?api_token=akijgroupadmin-abfd7bb4-2148-4ecd-81f8-295522b001f8&sid=AILERP&sms=" + SMS + "&msisdn=" + Phoneno + "&csms_id=4473433434pZ684333392";

                string SSLComerge = WebServices.CallWebService(SSLURI);

                return SSLComerge;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string CallWebService(string requestURL)
        {
            string response = "";
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                WebRequest myWebRequest = WebRequest.Create(requestURL);
                WebResponse myWebResponse = myWebRequest.GetResponse();
                Stream streamResponse = myWebResponse.GetResponseStream();
                StreamReader reader = new StreamReader(streamResponse);
                response = reader.ReadToEnd();
                reader.Close();
                streamResponse.Close();
                myWebResponse.Close();
            }
            catch (Exception ex)
            {
                response = ex.ToString();
            }
            return response;
        }
    }
}
