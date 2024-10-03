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
using System.Linq;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Entities.User;
using Common.Utilities;

namespace Mahak.Api.Controllers.v1
{
    [ApiVersion("1")]
    public class ProvinceController : BaseController
    {
        private readonly IRepository<Province> _repository;

        private readonly IMapper _mapper;
        private readonly IRepository<UserProvince> _upRepository;
        private readonly UserManager<User> _userManager;

        public ProvinceController(IRepository<Province> repository,
                             IMapper mapper
             , IRepository<UserProvince> upRepository, UserManager<User> userManager)
        {
            _repository = repository;
            _mapper = mapper;
            _upRepository = upRepository;
            _userManager = userManager;
        }

        [HttpPost("[action]")]
        [Authorize]
        public virtual async Task<ApiResult<ProvinceDto>> EditProvince([FromBody] ProvinceDto entity, CancellationToken cancellationToken)
        {
            var dto = await _repository.TableNoTracking.SingleOrDefaultAsync(c => c.Id == entity.Id, cancellationToken);

            if (dto == null)
            {
                dto = new Province();
                dto.Id = Guid.Empty;
            }

            dto.Code = entity.Code;
            dto.Name = entity.Name;

            if (dto.Id == Guid.Empty)
            {
                var repeatList = _repository.TableNoTracking.Any(c => c.Name == entity.Name);
                if (repeatList)
                {
                    return BadRequest("نام استان تکراری می باشد.");
                }

                repeatList = _repository.TableNoTracking.Any(c => c.Code == entity.Code);
                if (repeatList)
                {
                    return BadRequest("کد استان تکراری می باشد.");
                }

                await _repository.AddAsync(dto, cancellationToken);
            }
            else
            {
                var repeatList = _repository.TableNoTracking.Any(c => c.Name == entity.Name && c.Id != entity.Id);
                if (repeatList)
                {
                    return BadRequest("نام استان تکراری می باشد.");
                }

                repeatList = _repository.TableNoTracking.Any(c => c.Code == entity.Code && c.Id != entity.Id);
                if (repeatList)
                {
                    return BadRequest("کد استان تکراری می باشد.");
                }

                await _repository.UpdateAsync(dto, cancellationToken);
            }

            var resultDto = await _repository.TableNoTracking.ProjectTo<ProvinceDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);

            return resultDto;
        }


        [HttpGet("[action]")]
        [Authorize]
        public async Task<ActionResult<List<ProvinceDto>>> GetProvinces(CancellationToken cancellationToken)
        {
            var list = await _repository.TableNoTracking.ProjectTo<ProvinceDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

            var userName = HttpContext.User.Identity.GetUserName();

            var user = await _userManager.FindByNameAsync(userName);
            var userId = 0;
            if (user != null)
            {
                if (user.UserName == "admin"  )
                {
                }
                else
                {
                    userId = user.Id;

                    var provinceIds = _upRepository.TableNoTracking.Where(c => c.UserId == userId).Select(c => c.ProvinceId).ToList();
                    list = list.Where(i => provinceIds.Contains(i.Id)).ToList();
                }
            }

            return Ok(list);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public virtual async Task<ApiResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var model = await _repository.GetByIdAsync(cancellationToken, id);

            if (model == null)
                return NotFound();

            await _repository.DeleteAsync(model, cancellationToken);

            return Ok();
        }
    }
}
