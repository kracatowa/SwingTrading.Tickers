using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tickers.Domain;
using Tickers.Domain.Intervals;

namespace Tickers.Infrastructure.EntityConfigurations
{
    public class CandleConfiguration : IEntityTypeConfiguration<Candle>
    {
        public void Configure(EntityTypeBuilder<Candle> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.ToTable("Candles");

            builder.Property(c => c.Date).IsRequired();
            builder.HasIndex(c => c.Date).IsUnique(false); 

            builder.Property(c => c.Open).IsRequired();
            builder.Property(c => c.High).IsRequired();
            builder.Property(c => c.Low).IsRequired();
            builder.Property(c => c.Close).IsRequired();
            builder.Property(c => c.Volume).IsRequired();
            builder.Property(c => c.Dividends).IsRequired();
            builder.Property(c => c.StockSplits).IsRequired();

            builder.HasOne<Interval>()
                .WithMany(i => i.Candles)
                .HasForeignKey(c => c.IntervalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
