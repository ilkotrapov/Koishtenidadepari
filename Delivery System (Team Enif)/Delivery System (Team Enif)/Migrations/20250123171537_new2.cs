using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery_System__Team_Enif_.Migrations
{
    /// <inheritdoc />
    public partial class new2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Pishka",
                table: "Packages",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ProductPrice",
                table: "Packages",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pishka",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "ProductPrice",
                table: "Packages");
        }
    }
}
