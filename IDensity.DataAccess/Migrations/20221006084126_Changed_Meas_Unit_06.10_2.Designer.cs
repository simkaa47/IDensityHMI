﻿// <auto-generated />
using IDensity.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IDensity.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20221006084126_Changed_Meas_Unit_06.10_2")]
    partial class Changed_Meas_Unit_0610_2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.29");

            modelBuilder.Entity("IDensity.DataAccess.Models.MeasUnit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DeviceType")
                        .HasColumnType("INTEGER");

                    b.Property<float>("K")
                        .HasColumnType("REAL");

                    b.Property<int>("Mode")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<float>("Offset")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("MeasUnits");
                });

            modelBuilder.Entity("IDensity.DataAccess.Models.MeasUnitMemory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("MeasUnitId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MeasUnitMemories");
                });
#pragma warning restore 612, 618
        }
    }
}
