using Microsoft.EntityFrameworkCore.Migrations;

namespace Vega.Migrations
{
    public partial class SeedFeaturesTableData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Features (Name) VALUES ('feature1')");
            migrationBuilder.Sql("INSERT INTO Features (Name) VALUES ('feature2')");
            migrationBuilder.Sql("INSERT INTO Features (Name) VALUES ('feature3')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Features WHERE NAME IN ('feature1', 'feature2', 'feature3')");
        }
    }
}
