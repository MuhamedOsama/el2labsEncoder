﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using SmartPower.Data;
using System;

namespace SmartPower.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20190915165604_mi1")]
    partial class mi1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SmartPower.Data.Tables.JobOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("EndDate");

                    b.Property<int>("JobOrderId");

                    b.Property<string>("MachineCode");

                    b.Property<DateTime>("StartDate");

                    b.Property<decimal>("TotalLength");

                    b.HasKey("Id");

                    b.HasIndex("JobOrderId")
                        .IsUnique();

                    b.ToTable("jobOrder");
                });

            modelBuilder.Entity("SmartPower.Data.Tables.Reading", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("JobOrderId");

                    b.Property<decimal>("Length");

                    b.Property<string>("MachineCode");

                    b.Property<bool>("status");

                    b.Property<DateTime>("time");

                    b.HasKey("Id");

                    b.ToTable("Reading");
                });

            modelBuilder.Entity("SmartPower.Data.Tables.JobOrder", b =>
                {
                    b.HasOne("SmartPower.Data.Tables.Reading", "Reading")
                        .WithOne("JobOrder")
                        .HasForeignKey("SmartPower.Data.Tables.JobOrder", "JobOrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
