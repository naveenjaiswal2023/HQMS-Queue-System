using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HQMS.API.Migrations
{
    /// <inheritdoc />
    public partial class AddHospitalRelationToDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HospitalId",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HospitalId",
                table: "Departments",
                column: "HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Hospitals_HospitalId",
                table: "Departments",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "HospitalId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Hospitals_HospitalId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_HospitalId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "Departments");
        }
    }
}
