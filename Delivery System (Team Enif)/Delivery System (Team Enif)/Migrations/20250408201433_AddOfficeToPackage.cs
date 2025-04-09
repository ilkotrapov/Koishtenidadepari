using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery_System__Team_Enif_.Migrations
{
    /// <inheritdoc />
    public partial class AddOfficeToPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OfficeId",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_OfficeId",
                table: "Packages",
                column: "OfficeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Offices_OfficeId",
                table: "Packages",
                column: "OfficeId",
                principalTable: "Offices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Package_Offices_OfficeId",
                table: "Package");

            migrationBuilder.DropIndex(
                name: "IX_Package_OfficeId",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "Package");
        }
    }
}
