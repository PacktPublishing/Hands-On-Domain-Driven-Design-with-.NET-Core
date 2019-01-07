using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Infrastructure
{
    public static class AppBuilderDatabaseExtensions
    {
        public static IApplicationBuilder EnsureDatabase(this IApplicationBuilder builder)
        {
            EnsureContextIsMigrated(builder.ApplicationServices.GetService<MarketplaceDbContext>());
//            EnsureContextIsMigrated(builder.ApplicationServices.GetService<UserProfileDbContext>());
            return builder;
        }

        private static void EnsureContextIsMigrated(DbContext context)
        {
            if (!context.Database.EnsureCreated())
                context.Database.Migrate();
        }

        public static IServiceCollection AddPostgresDbContext<T>(this IServiceCollection services, 
            string connectionString) where T : DbContext
        {
            services.AddDbContext<T>(options => options.UseNpgsql(connectionString));
            return services;
        }
    }
}