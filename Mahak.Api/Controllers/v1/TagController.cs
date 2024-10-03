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
    public class TagController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ILogger<TagController> _logger;
        private readonly IMediator _mediator;
        private readonly IRepository<Tag> _repository;

        public TagController(IMapper mapper,
                                  ILogger<TagController> logger,
                                  IMediator mediator,
            IRepository<Tag> repository)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
            
        }

        [HttpGet]
        [DynamicAuthorization(TagClaims.CreateEditTages)]
        public virtual async Task<ActionResult<List<TagDto>>> Get(CancellationToken cancellationToken)
        {
            List<TagDto> list = null;
            var tagList = _repository.TableNoTracking;
            
            list = await tagList.ProjectTo<TagDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);


            return Ok(list);
        }

        [HttpPost("[action]")]
        [Authorize]
        public virtual async Task<ApiResult<TagDto>> EditTag([FromBody] TagDto entity, CancellationToken cancellationToken)
        {
            var dto = await _repository.TableNoTracking.SingleOrDefaultAsync(c => c.Id == entity.Id, cancellationToken);

            if (dto == null)
            {
                dto = new Tag();
                dto.Id = Guid.Empty;
            }

            dto.Title = entity.Title;
            dto.TagGroupId = entity.TagGroupId;

            if (dto.Id == Guid.Empty)
            {
                var repeatList = _repository.TableNoTracking.Any(c => c.Title == entity.Title);
                if (repeatList)
                {
                    return BadRequest("نام تگ تکراری می باشد.");
                }

                await _repository.AddAsync(dto, cancellationToken);
            }
            else
            {
                var repeatList = _repository.TableNoTracking.Any(c => c.Title == entity.Title && c.Id != entity.Id);
                if (repeatList)
                {
                    return BadRequest("نام تگ تکراری می باشد.");
                }

                await _repository.UpdateAsync(dto, cancellationToken);
            }

            var resultDto = await _repository.TableNoTracking.ProjectTo<TagDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);

            return resultDto;
        }

        [HttpDelete("{id}")]
        [DynamicAuthorization(KioskClaims.DeleteKiosk)]
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
