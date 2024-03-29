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
    [Migration("20191115130942_AddedMachineEntityWithOneToManyRelationWithOrders")]
    partial class AddedMachineEntityWithOneToManyRelationWithOrders
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

                    b.Property<bool>("Consumed");

                    b.Property<DateTime?>("EndDate");

                    b.Property<int>("JobOrderId");

                    b.Property<string>("MachineCode");

                    b.Property<int?>("MachineId");

                    b.Property<DateTime>("StartDate");

                    b.Property<decimal>("TotalLength");

                    b.HasKey("Id");

                    b.HasIndex("MachineId");

                    b.ToTable("jobOrders");
                });

            modelBuilder.Entity("SmartPower.Data.Tables.Machine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("MachineCode")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("machines");
                });

            modelBuilder.Entity("SmartPower.Data.Tables.Reading", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("JobOrderId");

                    b.Property<decimal>("Length");

                    b.Property<string>("MachineCode");

                    b.Property<int>("status");

                    b.Property<DateTime>("time");

                    b.HasKey("Id");

                    b.HasIndex("JobOrderId")
                        .IsUnique();

                    b.ToTable("Reading");
                });

            modelBuilder.Entity("SmartPower.Data.Tables.JobOrder", b =>
                {
                    b.HasOne("SmartPower.Data.Tables.Machine")
                        .WithMany("jobOrders")
                        .HasForeignKey("MachineId");
                });

            modelBuilder.Entity("SmartPower.Data.Tables.Reading", b =>
                {
                    b.HasOne("SmartPower.Data.Tables.JobOrder", "JobOrder")
                        .WithOne("Reading")
                        .HasForeignKey("SmartPower.Data.Tables.Reading", "JobOrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
