using Common.Utilities;
using Entities;
using MediatR;

namespace Mahak.Api.Models.Orders.Queries.Pagination
{
    public class GetOrderFilter
    {
        public string CategoryIds { get; set; }
        public string ProvinceIds { get; set; }
        public string Tags { get; set; }

        public string Filter { get; set; }
        public string FieldName { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
        public SortType SortType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public long DateTime { get; set; }
        public int SearchType { get; set; }
        public bool ApplySum { get; set; }
    }
    public class GetOrderPaginationQuery : GetOrderFilter, IRequest<PagedModel<CartSelectDto>>
    {


    }

    public class GetOrderSummeryPaginationQuery : GetOrderFilter, IRequest<List<CartSummerySelectDto>>
    {

    }
}
