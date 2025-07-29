using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tickers.Domain;

namespace Tickers.Infrastructure.EntityConfigurations
{
    public class TickerConfiguration : IEntityTypeConfiguration<Ticker>
    {
        public void Configure(EntityTypeBuilder<Ticker> builder)
        {
            builder.HasKey(t => t.Symbol);

            builder.ToTable("Tickers");

            builder.Property(t => t.Symbol)
                .IsRequired()
                .HasMaxLength(15);

           builder.HasMany(t => t.Intervals)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);
        }

    }
}
