using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPower.Data.Tables
{
    public class Machine
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string MachineCode { get; set; }
        public virtual ICollection<JobOrder> jobOrders { get; set; }
    }
}
