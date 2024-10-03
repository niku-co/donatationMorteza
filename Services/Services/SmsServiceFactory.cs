using Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Services.Services.SmsProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public enum SmsServiceType
    {
        none = 0,
        kavenegar = 1,
        ADP = 2,
    }
    public interface ISmsSendingSrevice
    {
        Task<string> SendAsync(string mobile, SMSSetting setting);
        Task<string> SendAsync(string mobile,string message, SMSSetting setting);
    }

    public class SmsServiceFactory
    {
        public static ISmsSendingSrevice GetSmsService(SmsServiceType smsServiceType)
        {

            if (smsServiceType == SmsServiceType.kavenegar)
            {
                return new KavenegarService();
            }
            else if (smsServiceType == SmsServiceType.ADP)
            {
                return new ADPService();
            }

            return null;
        }
    }
}
