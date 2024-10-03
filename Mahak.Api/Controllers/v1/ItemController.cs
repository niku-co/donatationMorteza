using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using Entities;
using Mahak.Api.Models;
using Mahak.Api.Models.Items.Queries.Pagination;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.DynamicAuthorization;
using Services.DynamicAuthorization.DTOs.Claims;
using Services.Services;
using WebFramework.Api;

namespace Mahak.Api.Controllers.v1;

[ApiVersion("1")]
public class ItemController : BaseController
{
    private readonly IRepository<Item> _repository;
    private readonly IRepository<Category> _cat_repository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IFileService _fileService;
    public ItemController(IRepository<Item> repository,
                          IRepository<Category> cat_repository,
                          IMapper mapper,
                           IFileService fileService,
                          IMediator mediator)
    {
        _repository = repository;
        _cat_repository = cat_repository;
        _mapper = mapper;
        _mediator = mediator;
        _fileService = fileService;
    }
    [HttpPost]
    [DynamicAuthorization(ServiceClaims.CreateService)]
    public virtual async Task<ApiResult<ItemSelectDto>> Create([FromForm] ItemDto dto, CancellationToken cancellationToken)
    {
        var cats = await _cat_repository.Entities.Where(j => dto.CategoriesId.Contains(j.Id)).ToListAsync(cancellationToken);
        if (cats == null)
            return NotFound();

        var model = dto.ToEntity(_mapper);

        model.Categories = cats;

        if (dto.Thumbnail is not null)
        {
            var image = await dto.Thumbnail.GetInputImageAsync(cancellationToken);
            model.ThumbnailPath = _fileService.Save(image, dto.Id.ToString(), ImageType.ServiceThumbnail);
        }

        await _repository.AddAsync(model, cancellationToken);

        var resultDto = await _repository.TableNoTracking.ProjectTo<ItemSelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

        return resultDto;
    }
    [HttpPut]
    [DynamicAuthorization(ServiceClaims.EditService)]
    public virtual async Task<ApiResult<ItemSelectDto>> Update(ItemDto dto, CancellationToken cancellationToken)
    {
        var cats = await _cat_repository.Entities.Where(j => dto.CategoriesId.Contains(j.Id)).ToListAsync(cancellationToken);
        if (cats == null)
            return NotFound();

        var model = await _repository.Entities.Include(i => i.Categories).Include(i => i.ItemTranslations)
            .SingleOrDefaultAsync(i => i.Id == dto.Id, cancellationToken: cancellationToken);
        if (model == null)
            return NotFound();

        model = dto.Adapt(model);
        model.ItemTranslations = dto.ItemTranslations.Adapt<ICollection<ItemTranslation>>();
        model.Categories = cats;

        //model.Active = dto.Active;
        //model.Special = dto.Special;
        //model.Price = dto.Price;
        //model.Priority = dto.Priority;
        //model.ShowTitle = dto.ShowTitle;
        //model.Title = dto.Title;
        //model.Description = dto.Description;
        //model.Categories.Clear();
        //model.Categories = cats;

        await _repository.UpdateAsync(model, cancellationToken);

        var resultDto = await _repository.TableNoTracking.ProjectTo<ItemSelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

        return resultDto;
    }

    [HttpPut("[action]")]
    [DynamicAuthorization(ServiceClaims.EditService)]
    public async Task<ApiResult<ItemSelectDto>> UpdateImage([FromForm] ItemThumbnail itemThumbnail, CancellationToken cancellationToken)
    {
        try
        {
            var model = await _repository.GetByIdAsync(cancellationToken, itemThumbnail.Id);
            if (model == null)
                return NotFound();

            if (itemThumbnail.File is not null)
            {
                var image = await itemThumbnail.File.GetInputImageAsync(cancellationToken);
                model.ThumbnailPath = _fileService.Save(image, itemThumbnail.Id.ToString(), ImageType.ServiceThumbnail);
            }
            else
            {
                _fileService.Delete($"{itemThumbnail.Id}_{ImageType.ServiceThumbnail}");
                model.ThumbnailPath = null;
            }
            await _repository.UpdateAsync(model, cancellationToken);

            var resultDto = await _repository.TableNoTracking.ProjectTo<ItemSelectDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

            return resultDto;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    [HttpDelete("[action]/{id}")]
    [DynamicAuthorization(ServiceClaims.EditService)]
    public async Task<ApiResult<ItemSelectDto>> DeleteImageAsync(int id, CancellationToken cancellationToken)
    {
        var model = await _repository.GetByIdAsync(cancellationToken, id);
        if (model == null)
            return NotFound();

        _fileService.Delete($"{id}_{ImageType.ServiceThumbnail}");
        model.ThumbnailPath = null;

        await _repository.UpdateAsync(model, cancellationToken);

        var resultDto = await _repository.TableNoTracking.ProjectTo<ItemSelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

        return resultDto;
    }

    [HttpDelete("{id}")]
    [DynamicAuthorization(ServiceClaims.DeleteService)]
    public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
    {
        var model = await _repository.GetByIdAsync(cancellationToken, id);

        if (model == null)
            return NotFound();

        await _repository.DeleteAsync(model, cancellationToken);

        return Ok();
    }

    [HttpGet("{id}")]
    [DynamicAuthorization(ServiceClaims.ServiceList)]
    public virtual async Task<ApiResult<ItemSelectDto>> Get(int id, CancellationToken cancellationToken)
    {
        var dto = await _repository.TableNoTracking.ProjectTo<ItemSelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

        if (dto == null)
            return NotFound();

        return dto;
    }

    [HttpGet("[action]")]
    [DynamicAuthorization(ServiceClaims.ServiceList)]
    public virtual async Task<ItemPaginationDto> GetPage([FromQuery] GetItemPaginationQuery paginationQuery, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(paginationQuery, cancellationToken);
        return result;
    }

    //[HttpPost("[action]")]
    //[DynamicAuthorization(DataMngClaims.EditDataMng)]
    //public virtual async Task<List<DataItemDto>> GetData([FromBody] GetDataItemQuery item, CancellationToken cancellationToken)
    //{
    //    var result = await _mediator.Send(item, cancellationToken);
    //    return result;
    //}

    //[HttpPost("[action]")]
    //[DynamicAuthorization(DataMngClaims.EditDataMng)]
    //public virtual async Task<ActionResult<EmdadBulkResponse>> SendDataMNG([FromBody] GetDataItemDetailQuery item, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var result = await _mediator.Send(item, cancellationToken);
    //        return result;
    //    }
    //    catch (Exception ex)
    //    {
    //       return BadRequest(ex.Message);
    //    }

    //}

    //[HttpPost("[action]")]
    //[DynamicAuthorization(DataMngClaims.EditDataMng)]
    //public virtual async Task<ActionResult<EmdadBulkResponse>> GetTraceData([FromBody] GetDataItemDetailQuery item, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var result = await _mediator.Send(item, cancellationToken);
    //        return result;
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }

    //}

}
