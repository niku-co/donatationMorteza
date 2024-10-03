using Entities;
using MediatR;

namespace Mahak.Api.Models.Items.Commands
{
    public class GetDataItemQuery : IRequest<List<DataItemDto>>
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int[] CategoryIds { get; set; }
        //public string FieldName { get; set; }
        //public int Limit { get; set; }
        //public int Page { get; set; }
        //public SortType SortType { get; set; }

    }

    public class GetDataItemDetailQuery : IRequest<EmdadBulkResponse>
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int[] CategoryIds { get; set; }
        //public string FieldName { get; set; }
        //public int Limit { get; set; }
        //public int Page { get; set; }
        //public SortType SortType { get; set; }

    }

    public class GetDataTraceQuery : IRequest<EmdadBulkTrace>
    {
        public string TraceId { get; set; }
    }
}

