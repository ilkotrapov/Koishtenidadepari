using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery_System__Team_Enif_.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Package_AspNetUsers_CreatedById",
                table: "Package");

            migrationBuilder.DropForeignKey(
                name: "FK_Package_DeliveryOptions_DeliveryOptionId",
                table: "Package");

            migrationBuilder.DropForeignKey(
                name: "FK_Package_DeliveryStatuses_DeliveryStatusId",
                table: "Package");

            migrationBuilder.DropForeignKey(
                name: "FK_Package_DeliveryTypes_DeliveryTypeId",
                table: "Package");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Package",
                table: "Package");

            migrationBuilder.RenameTable(
                name: "Package",
                newName: "Packages");

            migrationBuilder.RenameIndex(
                name: "IX_Package_DeliveryTypeId",
                table: "Packages",
                newName: "IX_Packages_DeliveryTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Package_DeliveryStatusId",
                table: "Packages",
                newName: "IX_Packages_DeliveryStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Package_DeliveryOptionId",
                table: "Packages",
                newName: "IX_Packages_DeliveryOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Package_CreatedById",
                table: "Packages",
                newName: "IX_Packages_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Packages",
                table: "Packages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_AspNetUsers_CreatedById",
                table: "Packages",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_DeliveryOptions_DeliveryOptionId",
                table: "Packages",
                column: "DeliveryOptionId",
                principalTable: "DeliveryOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_DeliveryStatuses_DeliveryStatusId",
                table: "Packages",
                column: "DeliveryStatusId",
                principalTable: "DeliveryStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_DeliveryTypes_DeliveryTypeId",
                table: "Packages",
                column: "DeliveryTypeId",
                principalTable: "DeliveryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_AspNetUsers_CreatedById",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_DeliveryOptions_DeliveryOptionId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_DeliveryStatuses_DeliveryStatusId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_DeliveryTypes_DeliveryTypeId",
                table: "Packages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Packages",
                table: "Packages");

            migrationBuilder.RenameTable(
                name: "Packages",
                newName: "Package");

            migrationBuilder.RenameIndex(
                name: "IX_Packages_DeliveryTypeId",
                table: "Package",
                newName: "IX_Package_DeliveryTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Packages_DeliveryStatusId",
                table: "Package",
                newName: "IX_Package_DeliveryStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Packages_DeliveryOptionId",
                table: "Package",
                newName: "IX_Package_DeliveryOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Packages_CreatedById",
                table: "Package",
                newName: "IX_Package_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Package",
                table: "Package",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Package_AspNetUsers_CreatedById",
                table: "Package",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Package_DeliveryOptions_DeliveryOptionId",
                table: "Package",
                column: "DeliveryOptionId",
                principalTable: "DeliveryOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Package_DeliveryStatuses_DeliveryStatusId",
                table: "Package",
                column: "DeliveryStatusId",
                principalTable: "DeliveryStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Package_DeliveryTypes_DeliveryTypeId",
                table: "Package",
                column: "DeliveryTypeId",
                principalTable: "DeliveryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
