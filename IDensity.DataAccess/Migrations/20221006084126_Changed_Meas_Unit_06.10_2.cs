using Microsoft.EntityFrameworkCore.Migrations;

namespace IDensity.DataAccess.Migrations
{
    public partial class Changed_Meas_Unit_0610_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Offset",
                table: "MeasUnits",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Offset",
                table: "MeasUnits");
        }
    }
}
