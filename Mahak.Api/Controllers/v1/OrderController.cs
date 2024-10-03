using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using Entities;
using Mahak.Api.Models;
using Mahak.Api.Models.Orders.Queries.Chart;
using Mahak.Api.Models.Orders.Queries.Export;
using Mahak.Api.Models.Orders.Queries.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFramework.Api;
using Services.DynamicAuthorization.DTOs.Claims;
using Services.DynamicAuthorization;
using Services.Services;
using System.Globalization;

namespace Mahak.Api.Controllers.v1;

[ApiVersion("1")]
public class OrderController : BaseController
{
    private readonly IRepository<Cart> _repository;
    private readonly IRepository<Category> _cat_repository;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly SmsService _smsService;

    public OrderController(IRepository<Cart> repository, IRepository<Category> cat_repository, IMediator mediator, IMapper mapper, SmsService smsService)
    {
        _repository = repository;
        _cat_repository = cat_repository;
        _mediator = mediator;
        _mapper = mapper;
        _smsService = smsService;
    }


    [HttpPost]
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public virtual async Task<ApiResult<CartSelectDto>> Create(CartDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var category = _cat_repository.Entities.SingleOrDefault(i => i.Id == dto.CategoryId);
            if (category == null)
                return NotFound();

            category.ErrorMessage = null;

            var model = dto.ToEntity(_mapper);
            model.Category = category;

            //var temp = await _repository.Entities.OrderByDescending(i => i.DateTime).FirstOrDefaultAsync(cancellationToken);

            //var lastRecord = temp.FirstOrDefault();

            //var lastRecord = await _repository.TableNoTracking.LastAsync();

            //var lastDate = lastRecord?.DateTime.ToShortLongFormat() ?? 0;

            //if (lastDate < currentDate)
            //    model.OrderNum = 1;
            //else
            //    model.OrderNum = (lastRecord?.OrderNum ?? 0) + 1;

            var currentDate = long.Parse(DateTime.UtcNow.ConvertTimeZone().ToString(DateTimeExtension.ShortFormat));

            model.OrderNum = GetOrderNumberForDay(DateTime.Now);

            _repository.BeginTransaction();

            await _cat_repository.UpdateAsync(category, cancellationToken);
            await _repository.AddAsync(model, cancellationToken);

            _repository.CommitTransaction();

            if (model.Paid && !string.IsNullOrEmpty(model.PhoneNumber))
            {
                try
                {
                    await this._smsService.SendAsync(model.PhoneNumber);
                }
                catch (Exception ex)
                {

                }
            }
            var resultDto = await _repository.TableNoTracking.ProjectTo<CartSelectDto>(_mapper.ConfigurationProvider)
                            .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

            return resultDto;
        }
        catch (Exception ex)
        {
            return null;
        }

    }
    [HttpGet("[action]")]
    [DynamicAuthorization(ReportClaims.Report)]
    public virtual async Task<PagedModel<CartSelectDto>> GetPage([FromQuery] GetOrderPaginationQuery paginationQuery, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(paginationQuery, cancellationToken);
        return result;
    }

    [HttpGet("[action]")]
    [DynamicAuthorization(ReportClaims.Report)]
    public virtual async Task<List<CartSummerySelectDto>> getPageSummery([FromQuery] GetOrderSummeryPaginationQuery paginationQuery, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(paginationQuery, cancellationToken);
        return result;
    }

    

    [HttpGet("[action]")]
    [DynamicAuthorization(ReportClaims.Report)]
    public virtual async Task<PagedModel<CartSelectDto>> GetSumPage([FromQuery] GetOrderPaginationQuery paginationQuery, CancellationToken cancellationToken)
    {
        paginationQuery.ApplySum = true;
        var result = await _mediator.Send(paginationQuery, cancellationToken);
        return result;
    }

    [HttpGet("[action]")]
    [DynamicAuthorization(ReportClaims.Report)]
    public virtual async Task<CartChartResult> GetChart([FromQuery] GetOrderChartQuery chartQuery, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(chartQuery, cancellationToken);
        return result;
    }
    [HttpGet("[action]")]
    [DynamicAuthorization(ReportClaims.Report)]
    public virtual async Task<IActionResult> Excel([FromQuery] GetOrderExcelQuery exportQuery, CancellationToken cancellationToken)
    {
        var content = await _mediator.Send(exportQuery, cancellationToken);

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Categories.xlsx");
    }

    [HttpGet("{id}")]
    [DynamicAuthorization(ReportClaims.Report)]
    public virtual async Task<ApiResult<CartSelectDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _repository.TableNoTracking.ProjectTo<CartSelectDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

        if (dto == null)
            return NotFound();

        return dto;
    }

    int GetOrderNumberForDay(DateTime currentDate)
    {
        var count = _repository.TableNoTracking.Count(order => EF.Property<DateTime>(order, "InsertTime").Date == currentDate.Date);
        return count + 1;
    }
}
