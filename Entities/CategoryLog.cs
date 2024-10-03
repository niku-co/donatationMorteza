using Entities.Common;
using System;

namespace Entities
{
    public class CategoryLog : BaseEntity<Guid>
    {
        public string DeviceId { get; set; }
        public int CategoryId { get; set; }
        public string Message { get; set; }
        public DateTime InsertTime { get; set; }
        public int LogType { get; set; }

        public Category Category { get; set; }

    }
}
