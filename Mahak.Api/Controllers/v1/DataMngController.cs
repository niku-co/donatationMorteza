using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contracts;
using Entities;
using Mahak.Api.Models;
using Mahak.Api.Models.Items.Commands;
using Mahak.Api.Models.Items.Queries.Pagination;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.DynamicAuthorization;
using Services.DynamicAuthorization.DTOs.Claims;
using WebFramework.Api;

namespace Mahak.Api.Controllers.v1;

[ApiVersion("1")]
public class DataMngController : BaseController
{

    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public DataMngController(
                          IMapper mapper,
                          IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }
    
    [HttpPost("[action]")]
    [DynamicAuthorization(DataMngClaims.EditDataMng)]
    public virtual async Task<List<DataItemDto>> GetData([FromBody] GetDataItemQuery item, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(item, cancellationToken);
        return result;
    }

    [HttpPost("[action]")]
    [DynamicAuthorization(DataMngClaims.EditDataMng)]
    public virtual async Task<ActionResult<EmdadBulkResponse>> SendDataMNG([FromBody] GetDataItemDetailQuery item, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(item, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
           return BadRequest(ex.Message);
        }

    }

    [HttpPost("[action]")]
    [DynamicAuthorization(DataMngClaims.EditDataMng)]
    public virtual async Task<ActionResult<EmdadBulkTrace>> GetTraceData([FromBody] GetDataTraceQuery item, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(item, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }


}
