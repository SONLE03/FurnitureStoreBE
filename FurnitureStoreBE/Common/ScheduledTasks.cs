using FurnitureStoreBE.Data;
using FurnitureStoreBE.Enums;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FurnitureStoreBE.Common
{
    public class ScheduledTasks
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<ScheduledTasks> _logger;
        public ScheduledTasks(ApplicationDBContext dbContext, ILogger<ScheduledTasks> logger)
        {
            _dbContext = dbContext;            _logger = logger;

        }
        public async Task UpdateCouponStatus()
        {
            _logger.LogInformation("Task handling");
            // Cập nhật trạng thái của các coupon hết hạn
            var expiredCoupons = await _dbContext.Coupons
                .Where(c => c.EndDate < DateTime.Now && c.ECouponStatus != ECouponStatus.Expired)
                .ToListAsync();

            foreach (var coupon in expiredCoupons)
            {
                coupon.ECouponStatus = ECouponStatus.Expired;
            }

            await _dbContext.SaveChangesAsync();
        }
        public async Task BackupDatabase()
        {
            var projectDirectory = Directory.GetCurrentDirectory(); 
            var backupFolderPath = Path.Combine(projectDirectory, "Backup"); 
            Directory.CreateDirectory(backupFolderPath);

            var backupFilePath = Path.Combine(backupFolderPath, $"FurnitureStoreBackup_{DateTime.Now:yyyyMMdd_HHmmss}.sql");

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "pg_dump",
                Arguments = $"-U postgres -h localhost -d FurnitureStore -f \"{backupFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();

                // Đọc thông tin từ stdout và stderr
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();

                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    _logger.LogInformation($"Database backup created at {backupFilePath}");
                }
                else
                {
                    _logger.LogError($"Error during backup: {error}");
                }
            }
        }
    }
}
