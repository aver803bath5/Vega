using Microsoft.EntityFrameworkCore.Migrations;

namespace Vega.Migrations
{
    public partial class RemoveFilePathColumnAndRenameRequestPathColumnToFileNameColumnInPhotosTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Photos");

            migrationBuilder.RenameColumn(
                name: "RequestPath",
                table: "Photos",
                newName: "FileName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Photos",
                newName: "RequestPath");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Photos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
