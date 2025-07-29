using Microsoft.EntityFrameworkCore;
using Tickers.Domain;
using Tickers.Domain.Intervals;
using Tickers.Infrastructure.EntityConfigurations;

namespace Tickers.Infrastructure
{
    /// <summary>
    /// For migrations, use this command :  dotnet ef migrations add InitialCreate --project Tickers.Infrastructure --startup-project Tickers.Api --context TickerContext
    /// </summary>
    /// <param name="options"></param>
    public class TickerContext(DbContextOptions<TickerContext> options) : DbContext(options)
    {
        public DbSet<Ticker> Tickers { get; set; }
        public DbSet<Interval> Intervals { get; set; }
        public DbSet<Candle> Candles { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply the TickerConfiguration
            modelBuilder.ApplyConfiguration(new TickerConfiguration());
            modelBuilder.ApplyConfiguration(new CandleConfiguration());
            modelBuilder.ApplyConfiguration(new IntervalConfiguration());
            modelBuilder.HasDefaultSchema("ticker");
        }
    }

}
