using Microsoft.EntityFrameworkCore;
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

            // Register DbContext for dependency injection  
            services.AddDbContext<TickerContext>(options =>
            {
                var currentLogLevel = builder.Configuration.GetValue<LogLevel>("Logging:LogLevel:Microsoft.EntityFrameworkCore");

                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                     .LogTo(Console.WriteLine, currentLogLevel);

                if (currentLogLevel == LogLevel.Debug)
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });


            // Apply EF migrations automatically using a hosted service  
            services.AddHostedService<MigrationHostedService>();

            // Register MediatR for dependency injection  
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<Program>();

                // cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            }
            );

            // Register ICandleFileService for dependency injection  
            services.AddScoped<ICandleFileService, CandleFileService>();
            services.AddScoped<ITickerRepository, TickerRepository>();
            services.AddScoped<ITickerQueries, TickerQueries>();

            var app = builder.Build();

            // Configure the HTTP request pipeline  
            if (app.Environment.IsEnvironment("Local") || app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();

            app.Run();
        }
    }

    public class MigrationHostedService(IServiceProvider serviceProvider) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TickerContext>();
            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
