using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contracts;
using Entities;
using Mahak.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Services.DynamicAuthorization.DTOs.Claims;
using Services.DynamicAuthorization;
using Services.Services;
using WebFramework.Api;
using Microsoft.EntityFrameworkCore;
using Common;
using System.Linq;

namespace Mahak.Api.Controllers.v1
{
    [ApiVersion("1")]
    public class SMSSettingController : BaseController
    {
        private readonly IRepository<SMSSetting> _repository;
        private readonly IFileService _fileService;

        //private readonly I_repository<SettingTranslation> settingTransRepo;
        private readonly IRepository<Language> langTransRepo;
        private readonly IMapper _mapper;

        public SMSSettingController(IRepository<SMSSetting> repository,
                             IFileService fileService,
                             IRepository<Language> langTransRepo,
                             IMapper mapper)
        {
            _repository = repository;
            _fileService = fileService;
            //this.settingTransRepo = settingTransRepo;
            this.langTransRepo = langTransRepo;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        [DynamicAuthorization(SmsServiceClaims.CreateEditSmsService)]
        public virtual async Task<ApiResult<SmsSettingDto>> GetSmsSetting(CancellationToken cancellationToken)
        {
            try
            {
                var dto = _repository.TableNoTracking.ProjectTo<SmsSettingDto>(_mapper.ConfigurationProvider)
                .FirstOrDefault();

                if (dto == null)
                    return NotFound();

                return dto;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        [HttpPost("[action]")]
        [DynamicAuthorization(SmsServiceClaims.CreateEditSmsService)]
        public virtual async Task<bool> EditSmsSetting([FromBody] SmsSettingDto entity, CancellationToken cancellationToken)
        {
            var dto = await _repository.TableNoTracking.SingleOrDefaultAsync(c => c.Id == entity.Id, cancellationToken);

            if (dto == null)
            {
                dto = new SMSSetting();
                dto.Id = Guid.Empty;
            }

            dto.SmsNumber = entity.SmsNumber;
            dto.AllowSendingSms = entity.AllowSendingSms;
            dto.SmsMessage = entity.SmsMessage;
            dto.APIKey = entity.APIKey;
           // dto.SmsUrl = entity.SmsUrl;
            dto.ServiceType = entity.ServiceType;
            dto.SmsUsername = entity.SmsUsername;
            dto.SmsPassword = entity.SmsPassword;

            if (dto.Id == Guid.Empty)
            {
                await _repository.AddAsync(dto, cancellationToken);
            }
            else
            {
                await _repository.UpdateAsync(dto, cancellationToken);
            }
            return true;
        }

    }
}
