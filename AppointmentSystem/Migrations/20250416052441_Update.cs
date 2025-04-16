using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailableTo",
                table: "Users",
                newName: "WorkDayStart");

            migrationBuilder.RenameColumn(
                name: "AvailableFrom",
                table: "Users",
                newName: "WorkDayEnd");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentDuration",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnavilableDays",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "AppointmentDuration", "UnavilableDays" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentDuration",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UnavilableDays",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "WorkDayStart",
                table: "Users",
                newName: "AvailableTo");

            migrationBuilder.RenameColumn(
                name: "WorkDayEnd",
                table: "Users",
                newName: "AvailableFrom");
        }
    }
}
