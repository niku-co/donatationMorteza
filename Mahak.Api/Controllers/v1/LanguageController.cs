using AutoMapper.QueryableExtensions;
using Data.Contracts;
using Entities;
using Mahak.Api.Models;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using IMapper = AutoMapper.IMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Mahak.Api.Controllers.v1;

[ApiVersion("1")]
public class LanguageController : BaseController
{
    private readonly IRepository<Language> _repository;
    private readonly IMapper _mapper;

    public LanguageController(IRepository<Language> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet("[Action]")]
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public async Task<ApiResult<IList<LanguageKioskDto>>> GetKioskLanguages(CancellationToken cancellationToken)
    {
        var dto = await _repository.TableNoTracking.ProjectTo<LanguageKioskDto>(_mapper.ConfigurationProvider)
            .Where(i => i.Active).ToListAsync(cancellationToken);

        if (dto == null)
            return NotFound();

        return dto;
    }
    [HttpGet("[Action]")]
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public async Task<ApiResult<IList<LanguageKioskDto>>> GetLanguages(CancellationToken cancellationToken)
    {
        var dto = await _repository.TableNoTracking.ProjectTo<LanguageKioskDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        if (dto == null)
            return NotFound();

        return dto;
    }

    [HttpPut("[Action]")]
    [Authorize(Roles = "Admin")]
    public async Task<ApiResult> Update([FromBody] IList<LanguageDto> dto, CancellationToken cancellationToken)
    {
        var ids = dto.Select(j => j.Id);
        var model = await _repository.Entities.Where(i => ids.Contains(i.Id))
            .ToListAsync(cancellationToken);

        if (model == null)
            return NotFound();

        model.ForEach(i =>
        {
            i.Active = dto.FirstOrDefault(j => j.Id == i.Id).Active;
        });

        await _repository.UpdateRangeAsync(model, cancellationToken);

        return Ok();
    }
}
