using FurnitureStoreBE.Data;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStoreBE.Configurations
{
    public static class DataHelper
    {

        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDBContext>();

            // Ghi lại chuỗi kết nối để gỡ lỗi
            var connectionString = dbContextSvc.Database.GetDbConnection().ConnectionString;
            Console.WriteLine($"Đang cố gắng kết nối bằng: {connectionString}");

            // Thực hiện migration: Đây là chương trình tương đương với Update-Database
            await dbContextSvc.Database.MigrateAsync();
        }
    }
}
