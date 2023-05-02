using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFilmField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "Films",
                newName: "MediaFileName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Films",
                newName: "FilmUrl");

            migrationBuilder.AddColumn<string>(
                name: "FilmName",
                table: "Films",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasSessions",
                table: "Films",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilmName",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "HasSessions",
                table: "Films");

            migrationBuilder.RenameColumn(
                name: "MediaFileName",
                table: "Films",
                newName: "Summary");

            migrationBuilder.RenameColumn(
                name: "FilmUrl",
                table: "Films",
                newName: "Name");
        }
    }
}
