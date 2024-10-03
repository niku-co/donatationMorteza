using MediatR;

namespace Mahak.Api.Models.Orders.Queries.Export
{
    public class GetOrderExcelQuery : IRequest<byte[]>
    {
        public string CategoryIds { get; set; }
        public string ProvinceIds { get; set; }
        public string Tags { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int SearchType { get; set; }

        public string Filter { get; set; }
    }
}
