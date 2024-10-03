using Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SMSSetting : BaseEntity<Guid>
    {
        public bool AllowSendingSms { get; set; }
        public string APIKey { get; set; }
        public string SmsNumber { get; set; }
        public string SmsMessage { get; set; }
        //public string SmsUrl { get; set; }
        public int ServiceType { get; set; }
        public string SmsUsername { get; set; }
        public string SmsPassword { get; set; }

        
    }
}
