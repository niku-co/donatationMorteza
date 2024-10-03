using System;
using System.Collections.Generic;
using Common;
using Entities.Common;

namespace Entities
{
    [Auditable]
    public class Cart : BaseEntity<Guid>
    {
        public long TotalPrice { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public Category Category { get; set; }
        public int OrderNum { get; set; }
        public string AffectiveAmount { get; set; }
        public string RefNum { get; set; }
        public string ResNum { get; set; }
        public string TerminalId { get; set; }
        public string TraceNumber { get; set; }
        public string CardNumber { get; set; }
        public long DateTime { get; set; }
        public bool ShoppingFree { get; set; }
        public bool Paid { get; set; } = false;
    }
}
