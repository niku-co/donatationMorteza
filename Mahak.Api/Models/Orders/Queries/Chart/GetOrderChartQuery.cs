using MediatR;

namespace Mahak.Api.Models.Orders.Queries.Chart
{
    public class GetOrderChartQuery : IRequest<CartChartResult>
    {
        //public ICollection<int> CategoryIds { get; set; }
        public string CategoryIds { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ProvinceIds { get; set; }
        public string Tags { get; set; }
        public int SearchType { get; set; }
        public string Filter { get; set; }

    }
}
