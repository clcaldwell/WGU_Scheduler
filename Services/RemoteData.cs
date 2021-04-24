
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace C969_Scheduler_WPF.Services
{

    public class DataContext : DbContext
    {
    }

    public class Data
    {
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = "server=wgudb.ucertify.com;" +
                "port=3306;" +
                "database=U05KQC;" +
                "user=U05KQC;" +
                "password=53688527502";

            services.AddDbContextPool<DataContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(connectionString)
                    .EnableSensitiveDataLogging() // These two calls are optional but help
                    .EnableDetailedErrors()      // with debugging (remove for production).
            );

        }
    }
}
