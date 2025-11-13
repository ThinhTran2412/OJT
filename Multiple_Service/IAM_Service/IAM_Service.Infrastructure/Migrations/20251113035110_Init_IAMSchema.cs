using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAM_Service.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init_IAMSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "iam_service");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "iam_service");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Roles",
                newSchema: "iam_service");

            migrationBuilder.RenameTable(
                name: "RolePrivileges",
                newName: "RolePrivileges",
                newSchema: "iam_service");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "RefreshTokens",
                newSchema: "iam_service");

            migrationBuilder.RenameTable(
                name: "Privileges",
                newName: "Privileges",
                newSchema: "iam_service");

            migrationBuilder.RenameTable(
                name: "PasswordResets",
                newName: "PasswordResets",
                newSchema: "iam_service");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "AuditLogs",
                newSchema: "iam_service");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                schema: "iam_service",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "iam_service",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "RolePrivileges",
                schema: "iam_service",
                newName: "RolePrivileges");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                schema: "iam_service",
                newName: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "Privileges",
                schema: "iam_service",
                newName: "Privileges");

            migrationBuilder.RenameTable(
                name: "PasswordResets",
                schema: "iam_service",
                newName: "PasswordResets");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                schema: "iam_service",
                newName: "AuditLogs");
        }
    }
}
