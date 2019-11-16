using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPower.Data.Tables
{
    public class JobOrder
    {
        [Key]
        public int Id { get; set; }
        public string JobOrderId { get; set; }
        public DateTime StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string MachineCode { get; set; }
        public decimal TotalLength { get; set; }
        public Reading Reading { get; set; }

    }
}
