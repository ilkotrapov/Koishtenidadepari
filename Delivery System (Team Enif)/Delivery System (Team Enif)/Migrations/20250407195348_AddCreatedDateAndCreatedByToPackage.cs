using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery_System__Team_Enif_.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedDateAndCreatedByToPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Package",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Package",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Package_CreatedById",
                table: "Package",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Package_AspNetUsers_CreatedById",
                table: "Package",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Package_AspNetUsers_CreatedById",
                table: "Package");

            migrationBuilder.DropIndex(
                name: "IX_Package_CreatedById",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Package");
        }
    }
}
