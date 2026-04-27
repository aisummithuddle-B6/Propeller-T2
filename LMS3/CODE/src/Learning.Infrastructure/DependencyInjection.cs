using Learning.Application.Ingestion;
using Learning.Application.Learners;
using Learning.Infrastructure.Ingestion;
using Learning.Infrastructure.Learners;
using Learning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Learning.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Database:Provider"] ?? "PostgreSql";

        services.AddDbContext<LearningDbContext>(options =>
        {
            if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                var sqlServerConnectionString = configuration.GetConnectionString("SqlServer")
                    ?? throw new InvalidOperationException("Connection string 'SqlServer' is not configured.");

                options.UseSqlServer(sqlServerConnectionString);
                return;
            }

            if (provider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase))
            {
                var postgresConnectionString = configuration.GetConnectionString("Postgres")
                    ?? throw new InvalidOperationException("Connection string 'Postgres' is not configured.");

                options.UseNpgsql(postgresConnectionString);
                return;
            }

            throw new InvalidOperationException(
                $"Unsupported database provider '{provider}'. Use 'PostgreSql' or 'SqlServer'.");
        });
        services.AddScoped<IDataIngestionService, EfDataIngestionService>();
        services.AddScoped<ILearnerProfileService, EfLearnerProfileService>();

        return services;
    }
}
