using System.Collections.Generic;
using MediatR;

namespace Mahak.Api.Models.Categories.Queries.Find
{
    public class GetByIdsQuery: IRequest<List<CategorySelectDto>>
    {
        public List<int> CategoryIds { get; set; }
    }
}
