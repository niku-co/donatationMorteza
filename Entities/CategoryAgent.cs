using Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CategoryAgent : BaseEntity<Guid>
    {
        public string FullName { get; set; }
        public string Code { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}
