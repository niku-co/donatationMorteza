using Entities;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.SmsProviders
{
    public class ADPService : ISmsSendingSrevice
    {
        public async Task<string> SendAsync(string mobile, SMSSetting setting)
        {
            return await SendAsync(mobile, setting.SmsMessage, setting);
        }

        public async Task<string> SendAsync(string mobile, string message, SMSSetting setting)
        {
            try
            {
                //http://ws.adpdigital.com/services/MessagingService?wsdl

                var num = $"98{mobile.TrimStart('0')}";

                JaxRpcMessagingServiceClient messagingService = new();
                var result = await messagingService.sendAsync(new(setting.SmsUsername, setting.SmsPassword, null, new[] { num }, null, null, null
                , 1, 2, true, DateTime.Now, message));


            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
    }
}
