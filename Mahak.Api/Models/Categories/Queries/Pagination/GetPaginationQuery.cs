using Common.Utilities;
using Entities;
using MediatR;

namespace Mahak.Api.Models.Categories.Queries.Pagination
{
    public class GetCategoryPaginationQuery : IRequest<CategoryPaginationDto>
    {
        public string Username { get; set; }
        public string Filter { get; set; }
        public string FieldName { get; set; }
        public string Tags { get; set; }
        public string Provinces { get; set; }
        public int Limit { get; set; }
        public int ActiveStatus { get; set; }
        public int Page { get; set; }
        public int SearchType { get; set; }

        public bool WithoutLatLong { get; set; }
        public SortType SortType { get; set; }
    }

}
