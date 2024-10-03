using Data.Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class SmsService
    {

        private readonly IRepository<SMSSetting> _repository;
        private readonly ILogger<SMSSetting> _logger;

        public SmsService(IRepository<SMSSetting> repository, ILogger<SMSSetting> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> SendAsync(string mobile)
        {
            string error = "";

            if (mobile.Length != 11)
            {
                error = "mobile is not valid : " + mobile;
                _logger.LogError(error);
                return error;
            }

            var dto = await _repository.TableNoTracking.FirstOrDefaultAsync();

            if (dto == null)
            {
                error = "sms setting is null";
                _logger.LogError(error);
                return error;
            }

            if (!dto.AllowSendingSms)
            {
                error = "AllowSendingSms is false";
                _logger.LogError(error);
                return error;
            }

            var smsService = SmsServiceFactory.GetSmsService((SmsServiceType)dto.ServiceType);

            if (smsService == null)
            {
                error = "smsService is null";
                _logger.LogError(error);
                return error;
            }

            error = await smsService.SendAsync(mobile, dto);

            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogError(error);

            }

            return error;
        }

        public async Task<string> SendAsync(string mobile,string message)
        {
            string error = "";

            if (mobile.Length != 11)
            {
                error = "mobile is not valid : " + mobile;
                _logger.LogError(error);
                return error;
            }

            var dto = await _repository.TableNoTracking.FirstOrDefaultAsync();

            if (dto == null)
            {
                error = "sms setting is null";
                _logger.LogError(error);
                return error;
            }

            if (!dto.AllowSendingSms)
            {
                error = "AllowSendingSms is false";
                _logger.LogError(error);
                return error;
            }

            var smsService = SmsServiceFactory.GetSmsService((SmsServiceType)dto.ServiceType);

            if (smsService == null)
            {
                error = "smsService is null";
                _logger.LogError(error);
                return error;
            }

            error = await smsService.SendAsync(mobile,message, dto);

            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogError(error);

            }

            return error;
        }

    }
}
