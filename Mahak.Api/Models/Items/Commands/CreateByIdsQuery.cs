using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using MediatR;

namespace Mahak.Api.Models.Items.Commands
{
    public class CreateByIdsQuery: IRequest<ItemSelectDto>
    {
        public List<int> Ints { get; set; }
    }
}
