using System.Data;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Common.Utilities;
using Data;
using Data.Contracts;
using Entities;
using Mahak.Api.Models.Items.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Category = Entities.Category;

namespace Mahak.Api.Models.DataMng.Queries;

public class GetDataDetailItemQueryHandler : IRequestHandler<GetDataItemDetailQuery, EmdadBulkResponse>
{
    private readonly IRepository<DataMNGSetting> _repository;
    private readonly ApplicationDbContext _dbContext;

    public GetDataDetailItemQueryHandler(IRepository<DataMNGSetting> repository, ApplicationDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;

    }
    private string add0(int digit)
    {
        return digit < 10 ? ("0" + digit) : (digit + "");
    }

    private long getLongDate(DateTime date)
    {
        return long.Parse((date.Year - 2000) + "" + add0(date.Month) + "" + add0(date.Day) + "" + add0(date.Hour) + "" + add0(date.Minute) + "" + add0(date.Second));
    }
    public async Task<EmdadBulkResponse> Handle(GetDataItemDetailQuery request, CancellationToken cancellationToken)
    {
        DateTime date = DateTime.Now;
        long? startDate = getLongDate(date);
        long? endtDate = startDate;

        var p = new PersianCalendar();
        if (!string.IsNullOrEmpty(request.StartDate))
        {
            var strDate = request.StartDate.Split("-");
            date = new DateTime(int.Parse(strDate[0]), int.Parse(strDate[1]), int.Parse(strDate[2]), p);
            startDate = getLongDate(date);
        }
        if (!string.IsNullOrEmpty(request.EndDate))
        {
            var strDate = request.EndDate.Split("-");
            date = new DateTime(int.Parse(strDate[0]), int.Parse(strDate[1]), int.Parse(strDate[2]), 23, 59, 0, p);
            endtDate = getLongDate(date);
        }

        try
        {
            var list = _dbContext.Set<Cart>().Include(x => x.CartItems).Include(d => d.Category)
           .Where(j => j.Paid && j.DateTime >= startDate && j.DateTime <= endtDate && request.CategoryIds.Contains(j.Category.Id))
           .SelectMany(x => x.CartItems, (k, s) => new
           {
               k.TotalPrice,
               s.ItemId,
               k.CardNumber,
               k.DateTime,
               k.PhoneNumber,
               k.RefNum,
               CategoryId = k.Category.Id,
           })
           .Join(_dbContext.Set<Category>().Include(c => c.Province), k => k.CategoryId, s => s.Id, (k, s) => new
           {
               k.TotalPrice,
               k.ItemId,
               k.CardNumber,
               k.DateTime,
               k.PhoneNumber,
               k.RefNum,
               s.ProvinceId,
               k.CategoryId,
               ProvinceCode = s.Province==null?0:s.Province.Code,
               s.Branch
           })
            .Join(_dbContext.Set<Item>(), k => k.ItemId, s => s.Id, (k, s) => new
            {
                k.TotalPrice,
                k.ItemId,
                k.CardNumber,
                k.DateTime,
                k.PhoneNumber,
                k.RefNum,
                k.ProvinceId,
                k.CategoryId,
                k.ProvinceCode,
                k.Branch,
                s.IntentionCode
            })
           .ToList();

          var dataListDto=list.Select(x => new DataDetailItemDto()
           {
               amount = x.TotalPrice,
               name_family = "",
               card_number = x.CardNumber,
               transaction_number = x.RefNum,
               provience = x.ProvinceCode == null ? "" : x.ProvinceCode.ToString(),
               Nationalcode = "",
               mobile = x.PhoneNumber,
               date_string = x.DateTime.ToShortStringFormatGarigory(),
               purpose_code = x.IntentionCode,
               source_id = "",
               time_string = x.DateTime.ToShortStringTimeFormatGarigory(),
               branch = x.Branch

           }); ;

            var setting = _repository.TableNoTracking.FirstOrDefault();
            EmdadService service = new EmdadService(setting);
            var data = await service.SendAsync(dataListDto.ToList());

            return data;
        }
        catch (Exception ex)
        {
            throw ex;
        }
       
    }
     

}