using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IDensity.DataAccess.Migrations
{
    public partial class Add_Meas_ResultLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasResultLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Time = table.Column<DateTime>(nullable: false),
                    Pulses = table.Column<float>(nullable: false),
                    CurValue1 = table.Column<float>(nullable: false),
                    AvgValue1 = table.Column<float>(nullable: false),
                    CurValue2 = table.Column<float>(nullable: false),
                    AvgValue2 = table.Column<float>(nullable: false),
                    HvValue1 = table.Column<float>(nullable: false),
                    Current1 = table.Column<float>(nullable: false),
                    Current2 = table.Column<float>(nullable: false),
                    Temperature = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasResultLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasResultLogs");
        }
    }
}
