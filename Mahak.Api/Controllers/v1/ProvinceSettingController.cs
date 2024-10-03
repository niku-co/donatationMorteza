using AutoMapper;
using Data.Contracts;
using Entities;
using Mahak.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.DynamicAuthorization.DTOs.Claims;
using Services.DynamicAuthorization;
using Services.Services;
using WebFramework.Api;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;

namespace Mahak.Api.Controllers.v1
{
    [ApiVersion("1")]
    public class ProvinceSettingController : BaseController
    {
        private readonly IRepository<ProvinceSettting> _repository;

        private readonly IMapper _mapper;

        public ProvinceSettingController(IRepository<ProvinceSettting> repository,
                             IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpPost("[action]")]
        [Authorize]
        public virtual async Task<bool> EditProvinceSetting([FromBody] ProvinceSetttingDto entity, CancellationToken cancellationToken)
        {
            var dto = await _repository.TableNoTracking.SingleOrDefaultAsync(c => c.Id == entity.Id, cancellationToken);

            if (dto == null)
            {
                dto = new ProvinceSettting();
                dto.Id = Guid.Empty;
            }

            dto.ApplyProvince = entity.ApplyProvince;
             
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


        [HttpGet("[action]")]
        [Authorize]
        public async Task<ApiResult<ProvinceSetttingDto>> GetProvinceSetting( CancellationToken cancellationToken)
        {
            try
            {
                var dto = _repository.TableNoTracking.ProjectTo<ProvinceSetttingDto>(_mapper.ConfigurationProvider)
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


    }
}
