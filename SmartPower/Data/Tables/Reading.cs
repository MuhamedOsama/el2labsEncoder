using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPower.Data.Tables
{
    public class Reading
    {
        [Key]
        public int Id { get; set; }
        public string MachineId { get; set; }
        public string PairId { get; set; }
        public int LineId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime LastRequest { get; set; }
        public decimal Length { get; set; }
        public short Status { get; set; }
        public short Assignment { get; set; }

    }
}
