using Entities.Common;
using System;
using System.Collections.Generic;

namespace Entities
{
    public class Province : BaseEntity<Guid>
    {
        public int Code { get; set; }
        public string Name { get; set; }

       // public ICollection<Category> Categories { get; set; }

    }
}
