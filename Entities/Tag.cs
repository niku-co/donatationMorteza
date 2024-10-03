using Entities.Common;
using System;
using System.Collections.Generic;

namespace Entities
{
    public class Tag : BaseEntity<Guid>
    {
        public Tag()
        {
            InsertTime = DateTime.Now;
        }
        public string Title { get; set; }
        public DateTime InsertTime { get; set; }

        public ICollection<Category> Categories { get; set; }
        public IList<CategoryTag> CategoryTags { get; } = new List<CategoryTag>();

        public int? TagGroupId { get; set; }
        public TagGroup? TagGroup { get; set; }

    }
}
