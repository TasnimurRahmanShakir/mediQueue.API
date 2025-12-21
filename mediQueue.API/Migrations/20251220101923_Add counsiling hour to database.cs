using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mediQueue.API.Migrations
{
    /// <inheritdoc />
    public partial class Addcounsilinghourtodatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scedule",
                table: "Appointments");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CounsilingEnd",
                table: "Doctors",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CounsilingStart",
                table: "Doctors",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateOnly>(
                name: "AppointmentDate",
                table: "Appointments",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "AppointmentTime",
                table: "Appointments",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CounsilingEnd",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "CounsilingStart",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "AppointmentDate",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AppointmentTime",
                table: "Appointments");

            migrationBuilder.AddColumn<DateTime>(
                name: "Scedule",
                table: "Appointments",
                type: "datetime2",
                nullable: true);
        }
    }
}
