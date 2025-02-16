using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery_System__Team_Enif_.Migrations
{
    /// <inheritdoc />
    public partial class bobermax2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Size",
                table: "Package",
                newName: "SizeR");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SizeR",
                table: "Package",
                newName: "Size");
        }
    }
}
