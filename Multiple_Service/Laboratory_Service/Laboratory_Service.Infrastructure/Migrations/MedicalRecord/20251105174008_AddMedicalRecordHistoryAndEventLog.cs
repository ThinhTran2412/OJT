using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Laboratory_Service.Infrastructure.Migrations.MedicalRecord
{
    /// <inheritdoc />
    public partial class AddMedicalRecordHistoryAndEventLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BatchNumber",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClinicalNotes",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessages",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstrumentUsed",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Interpretation",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LotNumber",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientName",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceRanges",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResultsDate",
                table: "MedicalRecords",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TestDate",
                table: "MedicalRecords",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TestResults",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TestType",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MedicalRecordHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MedicalRecordId = table.Column<int>(type: "integer", nullable: false),
                    SnapshotJson = table.Column<string>(type: "text", nullable: false),
                    ChangeSummary = table.Column<string>(type: "text", nullable: false),
                    ChangedBy = table.Column<string>(type: "text", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecordHistories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalRecordHistories");

            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "ClinicalNotes",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "ErrorMessages",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "InstrumentUsed",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Interpretation",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "LotNumber",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "PatientName",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "ReferenceRanges",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "ResultsDate",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "TestDate",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "TestResults",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "TestType",
                table: "MedicalRecords");
        }
    }
}
