using Microsoft.EntityFrameworkCore;
using Tickers.Domain;

namespace Tickers.Infrastructure.Repositories
{
    public class TickerRepository(TickerContext tickerContext) : ITickerRepository
    {
        private readonly TickerContext _tickerContext = tickerContext;

        public DbContext GetDbContext()
        {
            return _tickerContext;
        }

        public async Task AddTickerAsync(Ticker ticker, CancellationToken cancellationToken)
        {
            await _tickerContext.AddAsync(ticker, cancellationToken);
        }

        public async Task<Ticker?> GetTickerAsync(string tickerSymbol, CancellationToken cancellationToken)
        {
            return await _tickerContext.Tickers.Include(i => i.Intervals)
                                              .ThenInclude(c => c.Candles)
                                              .FirstOrDefaultAsync(ticker => ticker.Symbol == tickerSymbol, cancellationToken);
        }
    }
}
