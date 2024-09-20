using FurnitureStoreBE.Data;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStoreBE.Utils
{
    public class DatabaseMigrationUtil
    {
        public static void DataBaseMigrationInstallation(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<ApplicationDBContext>()
                    .Database.Migrate();
                
            }
        }
    }
}
