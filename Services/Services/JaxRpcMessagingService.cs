//using Common;
//using Microsoft.Extensions.Options;
//using ServiceReference1;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Services.Services
//{
//    public class JaxRpcMessagingService : ISmsService
//    {
//        private readonly SmsSettings _settings;

//        public JaxRpcMessagingService(IOptionsSnapshot<SiteSettings> settings)
//        {
//            _settings = settings.Value.SmsSettings;
//        }
//        public async Task<short> SendAsync(string message = "پیام تست از طرف محک", string phoneNumber = "")
//        {
//            try
//            {
//                JaxRpcMessagingServiceClient messagingService = new();
//                var result = await messagingService.sendAsync(new(_settings.User, _settings.Pass, null, new[] { phoneNumber ?? _settings.PhoneNumber }, null, null, null
//                    , 1, 2, true, DateTime.Now, message));

//                return result.sendReturn.status;
//            }
//            catch (Exception e)
//            {
//            }
//            return 0;
//        }
//    }
//}
