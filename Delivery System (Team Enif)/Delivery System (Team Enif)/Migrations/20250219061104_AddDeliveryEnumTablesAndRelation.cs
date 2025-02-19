using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery_System__Team_Enif_.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryEnumTablesAndRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryType",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Package");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryOptionId",
                table: "Package",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryStatusId",
                table: "Package",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryTypeId",
                table: "Package",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DeliveryOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Package_DeliveryOptionId",
                table: "Package",
                column: "DeliveryOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Package_DeliveryStatusId",
                table: "Package",
                column: "DeliveryStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Package_DeliveryTypeId",
                table: "Package",
                column: "DeliveryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Package_DeliveryOptions_DeliveryOptionId",
                table: "Package",
                column: "DeliveryOptionId",
                principalTable: "DeliveryOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Package_DeliveryStatuses_DeliveryStatusId",
                table: "Package",
                column: "DeliveryStatusId",
                principalTable: "DeliveryStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Package_DeliveryTypes_DeliveryTypeId",
                table: "Package",
                column: "DeliveryTypeId",
                principalTable: "DeliveryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Package_DeliveryOptions_DeliveryOptionId",
                table: "Package");

            migrationBuilder.DropForeignKey(
                name: "FK_Package_DeliveryStatuses_DeliveryStatusId",
                table: "Package");

            migrationBuilder.DropForeignKey(
                name: "FK_Package_DeliveryTypes_DeliveryTypeId",
                table: "Package");

            migrationBuilder.DropTable(
                name: "DeliveryOptions");

            migrationBuilder.DropTable(
                name: "DeliveryStatuses");

            migrationBuilder.DropTable(
                name: "DeliveryTypes");

            migrationBuilder.DropIndex(
                name: "IX_Package_DeliveryOptionId",
                table: "Package");

            migrationBuilder.DropIndex(
                name: "IX_Package_DeliveryStatusId",
                table: "Package");

            migrationBuilder.DropIndex(
                name: "IX_Package_DeliveryTypeId",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "DeliveryOptionId",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "DeliveryStatusId",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "DeliveryTypeId",
                table: "Package");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryType",
                table: "Package",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Package",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
