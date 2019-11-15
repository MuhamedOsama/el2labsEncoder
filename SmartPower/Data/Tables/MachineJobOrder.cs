using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPower.Data.Tables
{
    public class MachineJobOrder
    {
        [Key]
        public int Id { get; set; }
        public JobOrder jobOrder{ get; set; }
        public Machine machine { get; set; }

    }
}
