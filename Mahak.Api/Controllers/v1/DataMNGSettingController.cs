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
    public class DataMNGSettingController : BaseController
    {
        private readonly IRepository<DataMNGSetting> _repository;

        private readonly IMapper _mapper;

        public DataMNGSettingController(IRepository<DataMNGSetting> repository,
                                                          IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        [DynamicAuthorization(DataMngClaims.EditDataMng)]
        public virtual async Task<ApiResult<DataMNGSettingDto>> GetDataMNGSetting(CancellationToken cancellationToken)
        {
            try
            {
                var dto = _repository.TableNoTracking.ProjectTo<DataMNGSettingDto>(_mapper.ConfigurationProvider)
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
        [DynamicAuthorization(DataMngClaims.EditDataMng)]
        public virtual async Task<bool> EditDataMNGSetting([FromBody] DataMNGSettingDto entity, CancellationToken cancellationToken)
        {
            var dto = await _repository.TableNoTracking.SingleOrDefaultAsync(c => c.Id == entity.Id, cancellationToken);

            if (dto == null)
            {
                dto = new DataMNGSetting();
                dto.Id = Guid.Empty;
            }

            dto.APIUrl = entity.APIUrl;
            dto.Username = entity.Username;
            dto.Password = entity.Password;

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
