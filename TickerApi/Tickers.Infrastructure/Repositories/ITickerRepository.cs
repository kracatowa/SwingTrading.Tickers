using Tickers.Domain;

namespace Tickers.Infrastructure.Repositories
{
    public interface ITickerRepository : IRepository
    {
        Task AddTickerAsync(Ticker ticker, CancellationToken cancellationToken);
        Task<Ticker?> GetTickerAsync(string tickerSymbol, CancellationToken cancellationToken);
    }
}
