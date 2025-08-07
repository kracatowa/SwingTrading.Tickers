using Microsoft.EntityFrameworkCore;
using Tickers.Infrastructure;

namespace Tickers.Api.Services
{
    public class MigrationHostedService(IServiceProvider serviceProvider, ILogger<MigrationHostedService> logger) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TickerContext>();
            try
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
                logger.LogInformation("Database migration completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database migration failed.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
