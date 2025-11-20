using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Laboratory_Service.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventLogs",
                columns: table => new
                {
                    EventLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EventLogMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    OperatorName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLogs", x => x.EventLogId);
                });

            migrationBuilder.CreateTable(
                name: "FlaggingConfigs",
                columns: table => new
                {
                    FlaggingConfigId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ParameterName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Min = table.Column<double>(type: "double precision", nullable: false),
                    Max = table.Column<double>(type: "double precision", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlaggingConfigs", x => x.FlaggingConfigId);
                });

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

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdentifyNumber = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientId);
                });

            migrationBuilder.CreateTable(
                name: "ProcessedMessages",
                columns: table => new
                {
                    ProcessedMessageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SourceSystem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TestOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedMessages", x => x.ProcessedMessageId);
                });

            migrationBuilder.CreateTable(
                name: "RawBackups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawContent = table.Column<string>(type: "text", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawBackups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    MedicalRecordId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.MedicalRecordId);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestOrders",
                columns: table => new
                {
                    TestOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TestType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MedicalRecordId = table.Column<int>(type: "integer", nullable: false),
                    RunBy = table.Column<int>(type: "integer", nullable: true),
                    RunDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAiReviewEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestOrders", x => x.TestOrderId);
                    table.ForeignKey(
                        name: "FK_TestOrders_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "MedicalRecordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    TestResultId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Parameter = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ValueNumeric = table.Column<double>(type: "double precision", nullable: true),
                    ValueText = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ReferenceRange = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Instrument = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PerformedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResultStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PerformedByUserId = table.Column<int>(type: "integer", nullable: true),
                    ReviewedByUserId = table.Column<int>(type: "integer", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewNotes = table.Column<string>(type: "text", nullable: true),
                    Flag = table.Column<string>(type: "text", nullable: false),
                    FlaggedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FlaggedBy = table.Column<int>(type: "integer", nullable: true),
                    ReviewedByAI = table.Column<bool>(type: "boolean", nullable: false),
                    AiReviewedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    ConfirmedByUserId = table.Column<int>(type: "integer", nullable: true),
                    ConfirmedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.TestResultId);
                    table.ForeignKey(
                        name: "FK_TestResults_TestOrders_TestOrderId",
                        column: x => x.TestOrderId,
                        principalTable: "TestOrders",
                        principalColumn: "TestOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    CommentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    TestResultId = table.Column<int>(type: "integer", nullable: true),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comment_TestOrders_TestOrderId",
                        column: x => x.TestOrderId,
                        principalTable: "TestOrders",
                        principalColumn: "TestOrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comment_TestResults_TestResultId",
                        column: x => x.TestResultId,
                        principalTable: "TestResults",
                        principalColumn: "TestResultId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_TestOrderId",
                table: "Comment",
                column: "TestOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_TestResultId",
                table: "Comment",
                column: "TestResultId");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_CreatedOn",
                table: "EventLogs",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FlaggingConfigs_Gender",
                table: "FlaggingConfigs",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_FlaggingConfigs_IsActive",
                table: "FlaggingConfigs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FlaggingConfigs_TestCode",
                table: "FlaggingConfigs",
                column: "TestCode");

            migrationBuilder.CreateIndex(
                name: "IX_FlaggingConfigs_TestCode_Gender",
                table: "FlaggingConfigs",
                columns: new[] { "TestCode", "Gender" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecordHistories_ChangedAt",
                table: "MedicalRecordHistories",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecordHistories_MedicalRecordId",
                table: "MedicalRecordHistories",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_CreatedAt",
                table: "MedicalRecords",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_PatientId",
                table: "MedicalRecords",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Email",
                table: "Patients",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_IdentifyNumber",
                table: "Patients",
                column: "IdentifyNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PhoneNumber",
                table: "Patients",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedMessages_MessageId",
                table: "ProcessedMessages",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedMessages_ProcessedAt",
                table: "ProcessedMessages",
                column: "ProcessedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedMessages_TestOrderId",
                table: "ProcessedMessages",
                column: "TestOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TestOrders_MedicalRecordId",
                table: "TestOrders",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_TestOrders_OrderCode",
                table: "TestOrders",
                column: "OrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestOrders_Status",
                table: "TestOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestOrderId",
                table: "TestResults",
                column: "TestOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "EventLogs");

            migrationBuilder.DropTable(
                name: "FlaggingConfigs");

            migrationBuilder.DropTable(
                name: "MedicalRecordHistories");

            migrationBuilder.DropTable(
                name: "ProcessedMessages");

            migrationBuilder.DropTable(
                name: "RawBackups");

            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropTable(
                name: "TestOrders");

            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
