using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_entradas.Migrations
{
    /// <inheritdoc />
    public partial class CambiandonombredelatablauseraUsuariocomotambienelatributoUsuariodelatablaaUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Usuario",
                table: "User",
                newName: "User");

            migrationBuilder.RenameIndex(
                name: "IX_User_Usuario",
                table: "User",
                newName: "IX_User_User");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserEvent",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserEvent");

            migrationBuilder.RenameColumn(
                name: "User",
                table: "User",
                newName: "Usuario");

            migrationBuilder.RenameIndex(
                name: "IX_User_User",
                table: "User",
                newName: "IX_User_Usuario");
        }
    }
}
