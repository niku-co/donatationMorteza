using Common.Utilities;
using Mahak.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.DynamicAuthorization.DTOs.Claims;
using Services.DynamicAuthorization;
using WebFramework.Api;
using AutoMapper;
using Entities;
using Data.Contracts;
using MediatR;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Mahak.Api.Controllers.v1
{
    [ApiVersion("1")]
    public class TagGroupController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ILogger<TagGroupController> _logger;
        private readonly IMediator _mediator;
        private readonly IRepository<TagGroup> _repository;

        public TagGroupController(IMapper mapper,
                                  ILogger<TagGroupController> logger,
                                  IMediator mediator,
            IRepository<TagGroup> repository)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
            
        }

        [HttpGet]
        [DynamicAuthorization(KioskClaims.KioskList)]
        public virtual async Task<ActionResult<List<TagGroupDto>>> Get(CancellationToken cancellationToken)
        {
            List<TagGroupDto> list = null;
            var tagList = _repository.TableNoTracking;
            
            list = await tagList.ProjectTo<TagGroupDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);


            return Ok(list);
        }

        [HttpPost("[action]")]
        [Authorize]
        public virtual async Task<ApiResult<TagGroupDto>> EditTag([FromBody] TagGroupDto entity, CancellationToken cancellationToken)
        {
            var dto = await _repository.TableNoTracking.SingleOrDefaultAsync(c => c.Id == entity.Id, cancellationToken);

            if (dto == null)
            {
                dto = new TagGroup();
                dto.Id = 0;
            }

            dto.Title = entity.Title;

            if (dto.Id ==0)
            {
                var repeatList = _repository.TableNoTracking.Any(c => c.Title == entity.Title);
                if (repeatList)
                {
                    return BadRequest("نام دسته بندی تگ تکراری می باشد.");
                }

                await _repository.AddAsync(dto, cancellationToken);
            }
            else
            {
                var repeatList = _repository.TableNoTracking.Any(c => c.Title == entity.Title && c.Id != entity.Id);
                if (repeatList)
                {
                    return BadRequest("نام دسته بندی تگ تکراری می باشد.");
                }

                await _repository.UpdateAsync(dto, cancellationToken);
            }

            var resultDto = await _repository.TableNoTracking.ProjectTo<TagGroupDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);

            return resultDto;
        }

        [HttpDelete("{id}")]
        [DynamicAuthorization(KioskClaims.DeleteKiosk)]
        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var model = await _repository.GetByIdAsync(cancellationToken, id);

            if (model == null)
                return NotFound();

            await _repository.DeleteAsync(model, cancellationToken);

            return Ok();
        }
    }
}
