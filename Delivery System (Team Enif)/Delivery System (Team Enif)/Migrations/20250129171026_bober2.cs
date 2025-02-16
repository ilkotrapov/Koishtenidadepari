using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery_System__Team_Enif_.Migrations
{
    /// <inheritdoc />
    public partial class bober2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Couriers_CourierId",
                table: "Deliveries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Couriers",
                table: "Couriers");

            migrationBuilder.DropColumn(
                name: "CourierId",
                table: "Couriers");

            migrationBuilder.AlterColumn<int>(
                name: "CourierId",
                table: "Deliveries",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Couriers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Couriers",
                table: "Couriers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Couriers_CourierId",
                table: "Deliveries",
                column: "CourierId",
                principalTable: "Couriers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Couriers_CourierId",
                table: "Deliveries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Couriers",
                table: "Couriers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Couriers");

            migrationBuilder.AlterColumn<string>(
                name: "CourierId",
                table: "Deliveries",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourierId",
                table: "Couriers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Couriers",
                table: "Couriers",
                column: "CourierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Couriers_CourierId",
                table: "Deliveries",
                column: "CourierId",
                principalTable: "Couriers",
                principalColumn: "CourierId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
