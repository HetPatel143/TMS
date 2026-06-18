using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionsToRequestTypeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Options",
                table: "RequestTypeFields",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 1,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 2,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 3,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 4,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 5,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 6,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 7,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 8,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 9,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 10,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 11,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 12,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 13,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 14,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 15,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 16,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 17,
                column: "Options",
                value: null);

            migrationBuilder.UpdateData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 18,
                column: "Options",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Options",
                table: "RequestTypeFields");
        }
    }
}
