using Learning.DatabaseMigrator;
using Learning.Infrastructure;
using Learning.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(AppContext.BaseDirectory))
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection()
    .AddInfrastructure(configuration)
    .BuildServiceProvider();

await using var scope = services.CreateAsyncScope();
var dbContext = scope.ServiceProvider.GetRequiredService<LearningDbContext>();

var created = await dbContext.Database.EnsureCreatedAsync();
var provider = configuration["Database:Provider"] ?? "PostgreSql";

Console.WriteLine(created
    ? $"Database schema created using provider '{provider}'."
    : $"Database schema already exists using provider '{provider}'.");

await DatabaseSeeder.SeedAsync(dbContext);
