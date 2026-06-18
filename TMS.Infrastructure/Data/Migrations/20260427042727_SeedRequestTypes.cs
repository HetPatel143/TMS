using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRequestTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RequestTypes",
                columns: new[] { "RequestTypeId", "CreatedAt", "CreatedBy", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Request for new laptop or laptop replacement", true, "Laptop Request", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Request access to software applications", true, "Software Access Request", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Expense reimbursement request", true, "Reimbursement Request", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Request for leave/time off", true, "Leave Request", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Work from home request", true, "WFH Request", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "ApprovalWorkflows",
                columns: new[] { "WorkflowId", "ApprovalOrder", "CreatedAt", "IsActive", "RequestTypeId", "RoleId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 1, 2 },
                    { 2, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 1, 4 },
                    { 3, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 2, 2 },
                    { 4, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 2, 4 },
                    { 5, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, 2 },
                    { 6, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, 3 },
                    { 7, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 4, 2 },
                    { 8, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 4, 5 },
                    { 9, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, 2 }
                });

            migrationBuilder.InsertData(
                table: "RequestTypeFields",
                columns: new[] { "FieldId", "CreatedAt", "DisplayOrder", "FieldLabel", "FieldName", "FieldType", "IsActive", "IsRequired", "RequestTypeId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Laptop Type", "laptop_type", "Dropdown", true, true, 1 },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "Specification/Model", "specification", "Text", true, true, 1 },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Reason for Request", "reason", "Text", true, true, 1 }
                });

            migrationBuilder.InsertData(
                table: "RequestTypeFields",
                columns: new[] { "FieldId", "CreatedAt", "DisplayOrder", "FieldLabel", "FieldName", "FieldType", "IsActive", "RequestTypeId" },
                values: new object[] { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, "Needed By Date", "needed_by", "Date", true, 1 });

            migrationBuilder.InsertData(
                table: "RequestTypeFields",
                columns: new[] { "FieldId", "CreatedAt", "DisplayOrder", "FieldLabel", "FieldName", "FieldType", "IsActive", "IsRequired", "RequestTypeId" },
                values: new object[,]
                {
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Software Name", "software_name", "Text", true, true, 2 },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "Access Type", "access_type", "Dropdown", true, true, 2 },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Business Justification", "justification", "Text", true, true, 2 },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Expense Type", "expense_type", "Dropdown", true, true, 3 },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "Amount (₹)", "amount", "Number", true, true, 3 },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Expense Date", "expense_date", "Date", true, true, 3 },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, "Description", "description", "Text", true, true, 3 },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Leave Type", "leave_type", "Dropdown", true, true, 4 },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "From Date", "from_date", "Date", true, true, 4 },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "To Date", "to_date", "Date", true, true, 4 }
                });

            migrationBuilder.InsertData(
                table: "RequestTypeFields",
                columns: new[] { "FieldId", "CreatedAt", "DisplayOrder", "FieldLabel", "FieldName", "FieldType", "IsActive", "RequestTypeId" },
                values: new object[] { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, "Reason", "reason", "Text", true, 4 });

            migrationBuilder.InsertData(
                table: "RequestTypeFields",
                columns: new[] { "FieldId", "CreatedAt", "DisplayOrder", "FieldLabel", "FieldName", "FieldType", "IsActive", "IsRequired", "RequestTypeId" },
                values: new object[,]
                {
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "WFH Date", "wfh_date", "Date", true, true, 5 },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "Duration (Days)", "duration", "Number", true, true, 5 },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Reason", "reason", "Text", true, true, 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ApprovalWorkflows",
                keyColumn: "WorkflowId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ApprovalWorkflows",
                keyColumn: "WorkflowId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ApprovalWorkflows",
                keyColumn: "WorkflowId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ApprovalWorkflows",
                keyColumn: "WorkflowId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ApprovalWorkflows",
                keyColumn: "WorkflowId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ApprovalWorkflows",
                keyColumn: "WorkflowId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ApprovalWorkflows",
                keyColumn: "WorkflowId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ApprovalWorkflows",
                keyColumn: "WorkflowId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ApprovalWorkflows",
                keyColumn: "WorkflowId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "RequestTypeFields",
                keyColumn: "FieldId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 5);
        }
    }
}
