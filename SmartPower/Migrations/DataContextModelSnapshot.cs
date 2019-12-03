﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartPower.Data;

namespace SmartPower.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SmartPower.Data.Tables.Reading", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Assignment");

                    b.Property<DateTime>("EndTime");

                    b.Property<DateTime>("LastRequest");

                    b.Property<decimal>("Length");

                    b.Property<int>("LineId");

                    b.Property<string>("MachineId");

                    b.Property<string>("PairId");

                    b.Property<DateTime>("StartTime");

                    b.Property<short>("Status");

                    b.HasKey("Id");

                    b.ToTable("Readings");
                });

            modelBuilder.Entity("SmartPower.Data.Tables.ReadingsLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Assignment");

                    b.Property<DateTime>("EndTime");

                    b.Property<decimal>("Length");

                    b.Property<int>("LineId");

                    b.Property<string>("MachineId");

                    b.Property<string>("PairId");

                    b.Property<DateTime>("StartTime");

                    b.Property<short>("Status");

                    b.HasKey("Id");

                    b.ToTable("ReadingsLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
