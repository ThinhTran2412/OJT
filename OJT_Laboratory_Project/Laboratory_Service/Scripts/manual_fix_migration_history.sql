-- ====================================================================
-- Script SQL để FIX migration history - CHẠY SCRIPT NÀY TRƯỚC
-- ====================================================================
-- Vấn đề: Bảng EventLogs đã tồn tại trong schema laboratory_service
-- nhưng migration history table trống, nên EF Core cố tạo lại bảng → lỗi
-- Giải pháp: Insert migration history record để đánh dấu migration đã được apply
-- ====================================================================

-- BƯỚC 1: Kiểm tra bảng EventLogs có tồn tại trong schema laboratory_service không
SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1 FROM information_schema.tables 
            WHERE table_schema = 'laboratory_service' 
            AND table_name = 'EventLogs'
        ) 
        THEN '✓ Bảng EventLogs đã tồn tại trong schema laboratory_service'
        ELSE '✗ Bảng EventLogs KHÔNG tồn tại - cần chạy migration'
    END AS check_result;

-- BƯỚC 2: Tạo schema laboratory_service nếu chưa tồn tại
CREATE SCHEMA IF NOT EXISTS laboratory_service;

-- BƯỚC 3: Tạo migration history table trong schema laboratory_service (nếu chưa có)
CREATE TABLE IF NOT EXISTS laboratory_service."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

-- BƯỚC 4: Insert migration history record với migration ID MỚI
-- Điều này sẽ đánh dấu migration đã được apply, EF Core sẽ không chạy lại migration
-- LƯU Ý: Nếu bảng đã tồn tại, chỉ insert history record này
-- Nếu bảng chưa tồn tại, cần xóa schema cũ: DROP SCHEMA IF EXISTS laboratory_service CASCADE;
INSERT INTO laboratory_service."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251120221944_InitialCreate', '9.0.0')
ON CONFLICT ("MigrationId") DO NOTHING;

-- BƯỚC 5: Kiểm tra kết quả
SELECT 
    '✓ Migration History trong schema laboratory_service:' AS info,
    "MigrationId",
    "ProductVersion"
FROM laboratory_service."__EFMigrationsHistory"
ORDER BY "MigrationId";

-- ====================================================================
-- SAU KHI CHẠY SCRIPT NÀY:
-- Chạy fix_schema.bat - migration sẽ nhận ra migration đã được apply và skip
-- ====================================================================

