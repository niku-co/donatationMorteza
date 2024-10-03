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
    public class AgentController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AgentController> _logger;
        private readonly IMediator _mediator;
        private readonly IRepository<CategoryAgent> _repository;

        public AgentController(IMapper mapper,
                                  ILogger<AgentController> logger,
                                  IMediator mediator,
            IRepository<CategoryAgent> repository)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
            
        }

        [HttpGet]
        [DynamicAuthorization(AgentClaims.AgentList)]
        public virtual async Task<ActionResult<List<AgentDto>>> Get(CancellationToken cancellationToken)
        {
            List<AgentDto> list = null;
            var tagList = _repository.TableNoTracking;
            
            list = await tagList.ProjectTo<AgentDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);


            return Ok(list);
        }

        [HttpPost("[action]")]
        [Authorize]
        public virtual async Task<ApiResult<AgentDto>> EditAgent([FromBody] AgentDto entity, CancellationToken cancellationToken)
        {
            var dto = await _repository.TableNoTracking.SingleOrDefaultAsync(c => c.Id == entity.Id, cancellationToken);

            if (dto == null)
            {
                dto = new CategoryAgent();
                dto.Id = Guid.Empty;
            }

            dto.FullName = entity.FullName;
            dto.Code = entity.Code;
            dto.PhoneNumber = entity.PhoneNumber;

            if (dto.Id == Guid.Empty)
            {
                var repeatList = _repository.TableNoTracking.Any(c => c.PhoneNumber == entity.PhoneNumber);
                if (repeatList)
                {
                    return BadRequest("شماره همراه عامل تکراری می باشد.");
                }

                repeatList = _repository.TableNoTracking.Any(c => c.Code == entity.Code);
                if (repeatList)
                {
                    return BadRequest("کد عامل تکراری می باشد.");
                }


                await _repository.AddAsync(dto, cancellationToken);
            }
            else
            {
                var repeatList = _repository.TableNoTracking.Any(c => c.PhoneNumber == entity.PhoneNumber && c.Id != entity.Id);
                if (repeatList)
                {
                    return BadRequest("شماره همراه عامل تکراری می باشد.");
                }

                repeatList = _repository.TableNoTracking.Any(c => c.Code == entity.Code && c.Id != entity.Id);
                if (repeatList)
                {
                    return BadRequest("کد عامل تکراری می باشد.");
                }

                await _repository.UpdateAsync(dto, cancellationToken);
            }

            var resultDto = await _repository.TableNoTracking.ProjectTo<AgentDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);

            return resultDto;
        }

        [HttpDelete("{id}")]
        [DynamicAuthorization(AgentClaims.DeleteAgent)]
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
