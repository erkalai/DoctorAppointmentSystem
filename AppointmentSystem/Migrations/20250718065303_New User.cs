using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppointmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class NewUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AppointmentDuration", "AvailableDays", "Email", "FullName", "Password", "Phone", "Qualifications", "Role", "Specialization", "UnavilableDays", "WorkDayEnd", "WorkDayStart" },
                values: new object[,]
                {
                    { new Guid("19111111-1111-1111-1111-111111111111"), null, null, "d@hospital.com", "Doctor", "admin@123", "1234567894", null, "Doctor", null, null, null, null },
                    { new Guid("41111111-1111-1111-1111-111111111111"), null, null, "r@hospital.com", "Receptionist", "admin@123", "1234567891", null, "Receptionist", null, null, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("19111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("41111111-1111-1111-1111-111111111111"));
        }
    }
}
