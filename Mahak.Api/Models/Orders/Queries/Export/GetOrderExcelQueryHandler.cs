using System.Linq;
using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClosedXML.Excel;
using Common.Utilities;
using Data.Contracts;
using Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mahak.Api.Models.Orders.Queries.Export;

public class GetOrderExcelQueryHandler : IRequestHandler<GetOrderExcelQuery, byte[]>
{
    private readonly IMapper _mapper;
    private readonly ILogger<Cart> _logger;
    private readonly IRepository<Cart> _repository;

    public GetOrderExcelQueryHandler(IRepository<Cart> repository, IMapper mapper, ILogger<Cart> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<byte[]> Handle(GetOrderExcelQuery request, CancellationToken cancellationToken)
    {
        var catIds = JsonSerializer.Deserialize<int[]>(request.CategoryIds);

        var query = _repository.TableNoTracking;

        if (catIds.Any())
        {
            query = query.Include(d => d.Category).Include(c => c.CartItems).ThenInclude(b => b.Item).ThenInclude(o => o.ItemTranslations)
                .Where(i => catIds.Contains(i.Category.Id));
        }

        Guid[] tagIds = new List<Guid>().ToArray();
        if (request.Tags != null)
        {
            tagIds = JsonSerializer.Deserialize<Guid[]>(request.Tags);
            if (tagIds.Length > 0)
                query = query.Include(c => c.Category).ThenInclude(n => n.Tags).Where(c => c.Category.Tags.Any(t => tagIds.Contains(t.Id)));
        }

        var prIds = JsonSerializer.Deserialize<Guid[]>(request.ProvinceIds).ToList();

        if (prIds.Any())
        {
            query = query.Where(i => prIds.Contains(i.Category.ProvinceId ?? Guid.Empty));
        }

        _logger.LogInformation("request.FromDate", request.FromDate);
        _logger.LogInformation("request.ToDate", request.ToDate);

        if (!string.IsNullOrEmpty(request.FromDate))
        {
            var startDate = request.FromDate.GetLongDate();
            query = query.Where(i => i.DateTime / 1_000_000 >= startDate);
        }
        if (!string.IsNullOrEmpty(request.ToDate))
        {
            var endDate = request.ToDate.GetLongDate();
            query = query.Where(i => i.DateTime / 1_000_000 <= endDate);
        }

        if (request.SearchType > 0)
        {
            if (!string.IsNullOrEmpty(request.Filter))
            {
                if (request.SearchType == 1)
                    query = query.Where(i => i.RefNum.Contains(request.Filter));
                else if (request.SearchType == 2)
                    query = query.Where(i => i.CardNumber.Contains(request.Filter));
                else
                    query = query.Where(i => i.PhoneNumber.Contains(request.Filter));
            }

        }


        var dto = query.ProjectTo<CartSelectDto>(_mapper.ConfigurationProvider).ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Orders");
        worksheet.SetRightToLeft(true);
        var currentRow = 1;

        worksheet.Cell(currentRow, 1).Value = "نام کیوسک";
        worksheet.Cell(currentRow, 2).Value = "شماره مرجع";
        worksheet.Cell(currentRow, 3).Value = "شماره پیگیری";
        worksheet.Cell(currentRow, 4).Value = "تاریخ و زمان ثبت سفارش";
        worksheet.Cell(currentRow, 5).Value = "قیمت (ریال)";
        worksheet.Cell(currentRow, 6).Value = "خرید آزاد";
        worksheet.Cell(currentRow, 7).Value = "پرداخت شده";
        worksheet.Cell(currentRow, 8).Value = "شماره همراه";
        worksheet.Cell(currentRow, 9).Value = "شماره کارت";
        worksheet.Cell(currentRow, 10).Value = "نام خدمت";
        foreach (var order in dto)
        {
            currentRow++;
            worksheet.Cell(currentRow, 1).Value = order.Category.Title;
            worksheet.Cell(currentRow, 2).Value = order.RefNum;
            //worksheet.Cell(currentRow, 2).DataType = XLDataType.Text;
            worksheet.Cell(currentRow, 3).Value = order.TraceNumber;
            //worksheet.Cell(currentRow, 3).DataType = XLDataType.Text;
            worksheet.Cell(currentRow, 4).Value = order.DateTime.ToLongStringFormat();
            worksheet.Cell(currentRow, 5).Value = order.TotalPrice.ToString("#,#0").Replace(',', '،');
            worksheet.Cell(currentRow, 6).Value = order.ShoppingFree ? "بلی" : "خیر";
            worksheet.Cell(currentRow, 7).Value = order.Paid ? "بلی" : "خیر";
            worksheet.Cell(currentRow, 8).Value = order.PhoneNumber;
            worksheet.Cell(currentRow, 9).Value = order.CardNumber;
            worksheet.Cell(currentRow, 10).Value = order.CartItems.Any() ? order.CartItems.ToList()[0].Item.ItemTranslations.FirstOrDefault(c => c.LanguageId == 1)?.Title + " " + order.CartItems.ToList()[0].Item.Price.ToString("#,#0").Replace(',', '،') : "";
            //worksheet.Cell(currentRow, 8).DataType = XLDataType.Text;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();


        return Task.FromResult(content);
    }

}
