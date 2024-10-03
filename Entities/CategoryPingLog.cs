using Entities.Common;
using System;

namespace Entities
{
    public class CategoryPingLog : BaseEntity<Guid>
    {
        public string DeviceId { get; set; }
        public int CategoryId { get; set; }
        public long TotalRequestCounter { get; set; }
        public long SuccessCounter { get; set; }
        public DateTime InsertTime { get; set; }
        public DateTime Date { get; set; }
        public int TurnOnHours { get; set; }
        public Category Category { get; set; }

        public long PosTotalRequestCounter { get; set; }
        public long PosSuccessCounter { get; set; }

    }
}
