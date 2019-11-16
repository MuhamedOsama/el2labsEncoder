using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPower.Data.Tables
{
    [Table("Reading")]
    public class Reading
    {
       

        [Key]
        public int Id { get; set; }
        public string MachineCode { get; set; }
        public decimal Length { get; set; }
        public int status { get; set; }
        public DateTime time { get; set; }
        
        public ICollection<JobOrder> JobOrders { get; set; }


    }
}
