using Microsoft.EntityFrameworkCore;
using Tickers.Api.Commands.Behaviors;
using Tickers.Api.Queries;
using Tickers.Api.Services;
using Tickers.Infrastructure;
using Tickers.Infrastructure.Repositories;

namespace Tickers.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            // Add services to the container      
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Register ILogger for dependency injection    
            services.AddLogging();

            // Register DbContext for dependency injection      
            services.AddDbContext<TickerContext>(options =>
            {
                var currentLogLevel = builder.Configuration.GetValue<LogLevel>("Logging:LogLevel:Microsoft.EntityFrameworkCore");

                options.UseSqlServer(builder.Configuration.GetConnectionString("Sql"), sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                })
                .LogTo(Console.WriteLine, currentLogLevel);

                if (currentLogLevel == LogLevel.Debug)
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });
            TestSqlConnection(services);

            // Apply EF migrations automatically using a hosted service      
            services.AddHostedService<MigrationHostedService>();

            // Register MediatR for dependency injection      
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<Program>();
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });

            // Register ICandleFileService for dependency injection      
            services.AddScoped<ICandleFileService, CandleFileService>();
            services.AddScoped<ITickerRepository, TickerRepository>();
            services.AddScoped<ITickerQueries, TickerQueries>();

            // Add health checks    
            services.AddHealthChecks();

            var app = builder.Build();

            // Configure the HTTP request pipeline      
            if (app.Environment.EnvironmentName.StartsWith("Local", StringComparison.InvariantCultureIgnoreCase))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Map health check endpoint    
            app.MapHealthChecks("/health");

            app.MapControllers();

            app.Run();
        }
        private static void TestSqlConnection(IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<TickerContext>();
            try
            {
                dbContext.Database.CanConnect();
                logger.LogInformation("App can communicate with SQL server...");
                logger.LogInformation("Testing the SQL user and password");

                // Test SQL user and password      
                var connection = dbContext.Database.GetDbConnection();
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    logger.LogInformation("SQL user and password are valid.");
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SQL connection or authentication test failed.");
            }
        }

    }
}
