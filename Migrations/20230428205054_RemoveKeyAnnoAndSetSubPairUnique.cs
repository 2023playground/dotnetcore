using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet.Migrations
{
    /// <inheritdoc />
    public partial class RemoveKeyAnnoAndSetSubPairUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FilmSubscription_UserId",
                table: "FilmSubscription");

            migrationBuilder.CreateIndex(
                name: "IX_FilmSubscription_UserId_FilmId",
                table: "FilmSubscription",
                columns: new[] { "UserId", "FilmId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FilmSubscription_UserId_FilmId",
                table: "FilmSubscription");

            migrationBuilder.CreateIndex(
                name: "IX_FilmSubscription_UserId",
                table: "FilmSubscription",
                column: "UserId");
        }
    }
}
