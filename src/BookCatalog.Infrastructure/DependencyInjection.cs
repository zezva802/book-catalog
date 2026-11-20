using BookCatalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookCatalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("BookCatalog") ?? throw new InvalidOperationException("Connection string 'BookCatalog' is not configured.");

        services.AddDbContext<BookCatalogDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }
}