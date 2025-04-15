using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class Recreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AvailableDays", "AvailableFrom", "AvailableTo", "Email", "FullName", "Password", "Phone", "Qualifications", "Role", "Specialization" },
                values: new object[] { new Guid("733256cd-2786-48d1-8b4e-6b5c5141da70"), null, null, null, "admin@hospital.com", "System Admin", "admin123", "1234567890", null, "Admin", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("733256cd-2786-48d1-8b4e-6b5c5141da70"));
        }
    }
}
