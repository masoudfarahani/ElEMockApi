using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELE.MockApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestBodySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestBodySchema",
                table: "Endpoints",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestBodySchema",
                table: "Endpoints");
        }
    }
}
