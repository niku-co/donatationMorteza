using Entities;
using MediatR;

namespace Mahak.Api.Models.Items.Queries.Pagination
{
    public class GetItemPaginationQuery : IRequest<ItemPaginationDto>
    {
        //public ICollection<int> CategoryIds { get; set; }
        public string CategoryIds { get; set; }
        public string Filter { get; set; }
        public string FieldName { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
        public SortType SortType { get; set; }
    }


}
