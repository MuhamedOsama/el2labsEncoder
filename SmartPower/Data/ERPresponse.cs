using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPower.Data
{
    public class ERPresponse
    {
        public List<string> jobOrders { get; set; }
        public string AccessToken { get; set; }
        public string Status { get; set; }
    }
}
