using Microsoft.EntityFrameworkCore.Migrations;

namespace Vega.Migrations
{
    public partial class RenameFileNameColumnToFilePathInPhotosTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Photos",
                newName: "FilePath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Photos",
                newName: "FileName");
        }
    }
}
