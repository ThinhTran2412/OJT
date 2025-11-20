using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Laboratory_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Laboratory_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for FlaggingConfig operations.
    /// </summary>
    public class FlaggingConfigRepository : IFlaggingConfigRepository
    {
        private readonly AppDbContext _dbContext;

        public FlaggingConfigRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets the active flagging configuration for a specific test code and gender.
        /// Logic: 
        /// 1) Query 1 lần: Lấy tất cả configs của testCode (cả Gender=null và gender-specific)
        /// 2) Ưu tiên: Config chung (Gender=null) → nếu có thì dùng luôn, bỏ qua gender từ message
        /// 3) Nếu không có config chung → mới xét gender từ message để tìm config gender-specific
        /// Tối ưu: Chỉ 1 query thay vì 2 queries, xử lý logic trong memory.
        /// </summary>
        public async Task<FlaggingConfig?> GetActiveConfigAsync(string testCode, string? gender, CancellationToken cancellationToken = default)
        {
            // Bước 1: Query 1 lần - Lấy TẤT CẢ configs của testCode này (cả Gender=null và gender-specific)
            // Ví dụ: Với testCode="Hb" → lấy cả [Hb, Gender=null], [Hb, Gender=Male], [Hb, Gender=Female]
            var configs = await _dbContext.FlaggingConfigs
                .AsNoTracking()
                .Where(fc => fc.TestCode == testCode && fc.IsActive)
                .OrderByDescending(fc => fc.Version)  // Ưu tiên version mới nhất
                .ToListAsync(cancellationToken);

            // Bước 2: Ưu tiên config chung (Gender=null) - Nếu có thì dùng luôn, BỎ QUA gender từ message
            var generalConfig = configs.FirstOrDefault(c => c.Gender == null);
            if (generalConfig != null)
            {
                return generalConfig;  // Trả về ngay, không cần xét gender từ message
            }

            // Bước 3: Không có config chung → mới xét gender từ message
            // Nếu message không có gender → không tìm được config
            if (string.IsNullOrEmpty(gender))
            {
                return null;
            }

            // Tìm config theo gender cụ thể từ message (trong danh sách đã lấy về)
            return configs.FirstOrDefault(c => c.Gender == gender);
        }

        /// <summary>
        /// Gets all active flagging configurations.
        /// </summary>
        public async Task<List<FlaggingConfig>> GetAllActiveConfigsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.FlaggingConfigs
                .AsNoTracking()
                .Where(fc => fc.IsActive)
                .OrderBy(fc => fc.TestCode)
                .ThenBy(fc => fc.Gender ?? "ZZZ") // null ở cuối
                .ToListAsync(cancellationToken);
        }
    }
}

