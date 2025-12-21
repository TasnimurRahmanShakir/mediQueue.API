using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mediQueue.API.Migrations
{
    /// <inheritdoc />
    public partial class addanotherconstraincancelled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "AppointmentStatus",
                table: "Appointments");

            migrationBuilder.AddCheckConstraint(
                name: "AppointmentStatus",
                table: "Appointments",
                sql: "[Status] in ('Pending', 'In Progress', 'Completed', 'Cancelled')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "AppointmentStatus",
                table: "Appointments");

            migrationBuilder.AddCheckConstraint(
                name: "AppointmentStatus",
                table: "Appointments",
                sql: "[Status] in ('Pending', 'In Progress', 'Completed')");
        }
    }
}
