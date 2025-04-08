using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery_System__Team_Enif_.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryStatus",
                table: "Deliveries");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "Deliveries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeliveryOptionId",
                table: "Deliveries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryStatusId",
                table: "Deliveries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryTypeId",
                table: "Deliveries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveryOptionId",
                table: "Deliveries",
                column: "DeliveryOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveryStatusId",
                table: "Deliveries",
                column: "DeliveryStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveryTypeId",
                table: "Deliveries",
                column: "DeliveryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_DeliveryOptions_DeliveryOptionId",
                table: "Deliveries",
                column: "DeliveryOptionId",
                principalTable: "DeliveryOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_DeliveryStatuses_DeliveryStatusId",
                table: "Deliveries",
                column: "DeliveryStatusId",
                principalTable: "DeliveryStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_DeliveryTypes_DeliveryTypeId",
                table: "Deliveries",
                column: "DeliveryTypeId",
                principalTable: "DeliveryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_DeliveryOptions_DeliveryOptionId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_DeliveryStatuses_DeliveryStatusId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_DeliveryTypes_DeliveryTypeId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_DeliveryOptionId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_DeliveryStatusId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_DeliveryTypeId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryOptionId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryStatusId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryTypeId",
                table: "Deliveries");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryStatus",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
