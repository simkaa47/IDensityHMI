using Microsoft.EntityFrameworkCore.Migrations;

namespace IDensity.DataAccess.Migrations
{
    public partial class Add_Meas_Result_Sett_View : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasResultViewSetts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CurVisibility = table.Column<bool>(nullable: false),
                    AvgVisibility = table.Column<bool>(nullable: false),
                    TimeUnit = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasResultViewSetts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasResultViewSetts");
        }
    }
}
