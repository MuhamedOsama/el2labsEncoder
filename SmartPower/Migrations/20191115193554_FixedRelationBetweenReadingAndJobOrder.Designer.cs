﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartPower.Data;

namespace SmartPower.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20191115193554_FixedRelationBetweenReadingAndJobOrder")]
    partial class FixedRelationBetweenReadingAndJobOrder
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SmartPower.Data.Tables.JobOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("EndDate");

                    b.Property<string>("JobOrderId");

                    b.Property<string>("MachineCode");

                    b.Property<int?>("ReadingId");

                    b.Property<DateTime>("StartDate");

                    b.Property<decimal>("TotalLength");

                    b.HasKey("Id");

                    b.HasIndex("ReadingId");

                    b.ToTable("jobOrders");
                });

            modelBuilder.Entity("SmartPower.Data.Tables.Reading", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Length");

                    b.Property<string>("MachineCode");

                    b.Property<int>("status");

                    b.Property<DateTime>("time");

                    b.HasKey("Id");

                    b.ToTable("Reading");
                });

            modelBuilder.Entity("SmartPower.Data.Tables.JobOrder", b =>
                {
                    b.HasOne("SmartPower.Data.Tables.Reading", "Reading")
                        .WithMany("jobOrders")
                        .HasForeignKey("ReadingId");
                });
#pragma warning restore 612, 618
        }
    }
}
