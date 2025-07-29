using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tickers.Domain.Intervals;

namespace Tickers.Infrastructure.EntityConfigurations
{
    public class IntervalConfiguration : IEntityTypeConfiguration<Interval>
    {
        public void Configure(EntityTypeBuilder<Interval> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.ToTable("Intervals");

            builder.Property(i => i.IntervalType)
                .IsRequired()
                .HasMaxLength(10)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<IntervalTypes>(v)
                );

            builder.HasIndex(i => i.IntervalType)
                .IsUnique(false);

            builder.HasMany(t => t.Candles)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        }
    }
}
