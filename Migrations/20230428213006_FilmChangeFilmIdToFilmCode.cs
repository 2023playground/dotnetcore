using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet.Migrations
{
    /// <inheritdoc />
    public partial class FilmChangeFilmIdToFilmCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilmId",
                table: "Films",
                newName: "FilmCode");

            migrationBuilder.RenameIndex(
                name: "IX_Films_FilmId",
                table: "Films",
                newName: "IX_Films_FilmCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilmCode",
                table: "Films",
                newName: "FilmId");

            migrationBuilder.RenameIndex(
                name: "IX_Films_FilmCode",
                table: "Films",
                newName: "IX_Films_FilmId");
        }
    }
}
