using DocumentFormat.OpenXml.Office2010.ExcelAc;
using MediatR;
using System.Collections.Generic;

namespace Mahak.Api.Models.Orders.Queries.Export
{
    public class GetCategoryExcelQuery: IRequest<byte[]>
    {
        public string CategoryIds { get; set; }
        public string Filter { get; set; }
        public IList<string> FieldNames { get; set; }

    }
}
