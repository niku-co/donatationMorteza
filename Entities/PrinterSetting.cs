using Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class PrinterSetting : BaseEntity<Guid>
    {
        public bool ActivePrinter { get; set; }
        public string PrintText { get; set; }
        public string QRCodeUrl { get; set; }
        public string LogoPhysicalPath { get; set; }
    }
}
