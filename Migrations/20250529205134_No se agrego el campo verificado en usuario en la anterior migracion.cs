using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_entradas.Migrations
{
    /// <inheritdoc />
    public partial class Noseagregoelcampoverificadoenusuarioenlaanteriormigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstaVerificado",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstaVerificado",
                table: "User");
        }
    }
}
