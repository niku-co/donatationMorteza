using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClosedXML.Excel;
using Data.Contracts;
using Entities;
using MediatR;

namespace Mahak.Api.Models.Orders.Queries.Export;

public class GetCategoryExcelQueryHandler : IRequestHandler<GetCategoryExcelQuery, byte[]>
{
    private readonly IMapper _mapper;
    //private readonly ILogger<Cart> _logger;
    private readonly IRepository<Category> _repository;

    public GetCategoryExcelQueryHandler(IRepository<Category> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public Task<byte[]> Handle(GetCategoryExcelQuery request, CancellationToken cancellationToken)
    {
        var catIds = System.Text.Json.JsonSerializer.Deserialize<int[]>(request.CategoryIds);
        var exp = _repository.TableNoTracking.ProjectTo<CategorySelectDto>(_mapper.ConfigurationProvider);
        if (!string.IsNullOrEmpty(request.Filter))
            exp = exp.Where(i => i.Title.Contains(request.Filter));


        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Categories");
        worksheet.SetRightToLeft(true);
        var currentRow = 1;
        int currentColumn = 1;
        foreach (var fieldName in request.FieldNames)
        {
            if (fieldName.Equals(nameof(CategorySelectDto.Title), System.StringComparison.CurrentCultureIgnoreCase))
            {
                worksheet.Cell(currentRow, currentColumn).Value = "نام مجازی کیوسک";
                currentColumn++;
            }
            else if (fieldName.Equals(nameof(CategorySelectDto.DeviceId), System.StringComparison.CurrentCultureIgnoreCase))
            {
                worksheet.Cell(currentRow, currentColumn).Value = "نام دستگاه";
                currentColumn++;
            }
            else if (fieldName.Equals(nameof(CategorySelectDto.Ip), System.StringComparison.CurrentCultureIgnoreCase))
            {
                worksheet.Cell(currentRow, currentColumn).Value = "ip دستگاه";
                currentColumn++;
            }
            else if (fieldName.Equals(nameof(CategorySelectDto.TerminalNo), System.StringComparison.CurrentCultureIgnoreCase))
            {
                worksheet.Cell(currentRow, currentColumn).Value = "شماره ترمینال";
                currentColumn++;
            }
            else if (fieldName.Equals(nameof(CategorySelectDto.LastActive), System.StringComparison.CurrentCultureIgnoreCase))
            {
                worksheet.Cell(currentRow, currentColumn).Value = "آخرین وضعیت";
                currentColumn++;
            }
            else if (fieldName.Equals(nameof(CategorySelectDto.ErrorMessage), System.StringComparison.CurrentCultureIgnoreCase))
            {
                worksheet.Cell(currentRow, currentColumn).Value = "توضیحات";
                currentColumn++;
            }
        }
        currentColumn = 1;
        foreach (var item in exp)
        {
            currentRow++;

            foreach (var fieldName in request.FieldNames)
            {
                if (fieldName.Equals(nameof(CategorySelectDto.Title), System.StringComparison.CurrentCultureIgnoreCase))
                {
                    worksheet.Cell(currentRow, currentColumn).Value = item.Title;
                    //worksheet.Cell(currentRow, currentColumn).DataType = XLDataType.Text;
                    currentColumn++;
                }
                if (fieldName.Equals(nameof(CategorySelectDto.DeviceId), System.StringComparison.CurrentCultureIgnoreCase))
                {
                    worksheet.Cell(currentRow, currentColumn).Value = item.DeviceId;
                    //worksheet.Cell(currentRow, currentColumn).DataType = XLDataType.Text;
                    currentColumn++;
                }
                if (fieldName.Equals(nameof(CategorySelectDto.Ip), System.StringComparison.CurrentCultureIgnoreCase))
                {
                    worksheet.Cell(currentRow, currentColumn).Value = item.Ip;
                    //worksheet.Cell(currentRow, currentColumn).DataType = XLDataType.Text;
                    currentColumn++;
                }
                if (fieldName.Equals(nameof(CategorySelectDto.TerminalNo), System.StringComparison.CurrentCultureIgnoreCase))
                {
                    worksheet.Cell(currentRow, currentColumn).Value = item.TerminalNo;
                    //worksheet.Cell(currentRow, currentColumn).DataType = XLDataType.Text;
                    currentColumn++;
                }
                if (fieldName.Equals(nameof(CategorySelectDto.LastActive), System.StringComparison.CurrentCultureIgnoreCase))
                {
                    worksheet.Cell(currentRow, currentColumn).Value = item.LastActive;
                    //worksheet.Cell(currentRow, currentColumn).DataType = XLDataType.Text;
                    currentColumn++;
                }
                if (fieldName.Equals(nameof(CategorySelectDto.ErrorMessage), System.StringComparison.CurrentCultureIgnoreCase))
                {
                    worksheet.Cell(currentRow, currentColumn).Value = item.ErrorMessage;
                    //worksheet.Cell(currentRow, currentColumn).DataType = XLDataType.Text;
                    currentColumn++;
                }
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();


        return Task.FromResult(content);
    }
}