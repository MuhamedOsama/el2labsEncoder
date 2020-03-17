using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoder.Data.Tables
{
    public class GenericReading
    {
        public int Id { get; set; }
        public string MachineId { get; set; }
        public int LineId { get; set; }
        public DateTime TimeStamp { get; set; }
        public decimal Length { get; set; }
        public string PairId { get; set; }
    }
}
