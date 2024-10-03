using Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class TagGroup : BaseEntity<int>
    {
        public string Title { get; set; }

        public ICollection<Tag> Tags { get; set; }

    }
}
