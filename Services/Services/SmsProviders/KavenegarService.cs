using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.SmsProviders
{
    public class KavenegarService : ISmsSendingSrevice
    {
        public async Task<string> SendAsync(string mobile, SMSSetting setting)
        {

            return await SendAsync(mobile, setting.SmsMessage, setting);
        }

        public async Task<string> SendAsync(string mobile, string message, SMSSetting setting)
        {
            var _client = new HttpClient();
            _client.BaseAddress = new Uri("https://api.kavenegar.com");

            string url = "/v1/" + setting.APIKey + "/sms/send.json?receptor=" + mobile + "&sender=" + setting.SmsNumber + "&message=" + System.Web.HttpUtility.UrlEncode(message);

            var response = await _client.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
            else
            {
                return (int)response.StatusCode + " : status. sending sms faild.";
            }

            return "";
        }


    }
}
