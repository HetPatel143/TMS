using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDemoUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "PasswordHash", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "rahul.sharma@tms.com", "Rahul", true, "Sharma", "$2a$11$tZUQUcvbLH5xDr6mk99fD.KnrKkF3piGTY8UuuSVGQKTDPN9WI3b6", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "priya.patel@tms.com", "Priya", true, "Patel", "$2a$11$yuecjY2kxPzGZB.fB4f3KurxIhzg4zvu/I.r8wpOeN2SkfRVEXxcC", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "vikram.singh@tms.com", "Vikram", true, "Singh", "$2a$11$ZZJNUHcuBYQlGcm5ytswi.247d7dAQ8KBu9ulgqQ4wncFnHKgoPJS", 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "anita.desai@tms.com", "Anita", true, "Desai", "$2a$11$5NcADmc3A.VXjtHIP5Tw9.eXlu5O0hsMyJ/QRTmkPP6O5vdzWlae.", 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "rajesh.kumar@tms.com", "Rajesh", true, "Kumar", "$2a$11$Sb4gn6BFeFYB9B6DxxyZHORJlXzfvfXyDHoUcr3H4o.z0xcCx60VO", 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "sneha.gupta@tms.com", "Sneha", true, "Gupta", "$2a$11$6prRBVopmQ3VIvud1zvAYutFVBweVWvNTSRPv8lEMjgraRMbigGJu", 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7);
        }
    }
}
