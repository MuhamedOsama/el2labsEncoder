using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPower.Data
{
    public class ERPresponse
    {
        public List<jobOrderResponse> jobOrders { get; set; }
        public string statusCode { get; set; }
        public string message { get; set; }
    }
}
