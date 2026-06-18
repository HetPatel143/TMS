using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestNumberToRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestNumber",
                table: "Requests",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.Sql(
                """
                WITH NumberedRequests AS
                (
                    SELECT
                        RequestId,
                        CreatedByUserId,
                        ROW_NUMBER() OVER (PARTITION BY CreatedByUserId ORDER BY CreatedAt, RequestId) AS UserSequence
                    FROM Requests
                )
                UPDATE r
                SET RequestNumber = CONCAT('USR-', nr.CreatedByUserId, '-', RIGHT(CONCAT('0000', nr.UserSequence), 4))
                FROM Requests r
                INNER JOIN NumberedRequests nr ON r.RequestId = nr.RequestId;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "RequestNumber",
                table: "Requests",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestNumber",
                table: "Requests",
                column: "RequestNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Requests_RequestNumber",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RequestNumber",
                table: "Requests");
        }
    }
}
