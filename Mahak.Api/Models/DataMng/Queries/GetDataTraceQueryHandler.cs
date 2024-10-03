using System.Data;
using System.Globalization;
using Data;
using Data.Contracts;
using Data.Repositories;
using Entities;
using Mahak.Api.Models.Items.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Category = Entities.Category;
using Item = Entities.Item;

namespace Mahak.Api.Models.DataMng.Queries;

public class GetDataTraceQueryHandler : IRequestHandler<GetDataTraceQuery, EmdadBulkTrace>
{
    private readonly IRepository<DataMNGSetting> _repository;
    public GetDataTraceQueryHandler(IRepository<DataMNGSetting> repository)
    {
    _repository = repository;
    }
    
    public async Task<EmdadBulkTrace> Handle(GetDataTraceQuery request, CancellationToken cancellationToken)
    {
        
        var setting = _repository.TableNoTracking.FirstOrDefault();
        EmdadService service = new EmdadService(setting);
        var data = await service.TraceAsync(request.TraceId);


        return await Task.FromResult(data);
    }
}