using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartPower.Data.Tables;

namespace SmartPower.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Reading> LengthReadings { get; set; }
        public DbSet<ReadingsLog> LengthReadingsLogs { get; set; }
        //public DbSet<Machine> machines { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            //modelBuilder.Entity<Reading>()
            //.HasOne<JobOrder>(s => s.JobOrder)
            //.WithOne(ad => ad.Reading)
            //.HasForeignKey<JobOrder>(ad => ad.JobOrderId);
            
        }
    }
}
