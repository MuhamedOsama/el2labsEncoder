using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPower.Data
{
    public class jobOrderResponse
    {
        public string machineId { get; set; }
        public string jobOrderId { get; set; }
        public string jobOrderNumber { get; set; }
        public int assemblyQty { get; set; }
    }
}
