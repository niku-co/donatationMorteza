using Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class DataMNGSetting : BaseEntity<Guid>
    {
        public string APIUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
