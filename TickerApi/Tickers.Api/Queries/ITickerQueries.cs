using Tickers.Domain;
using Tickers.Domain.Intervals;

namespace Tickers.Api.Queries
{
    public interface ITickerQueries
    {
        Task<List<TickerQueries.SymbolPeriodChecker>> GetTickersNeedingCandleUpdates(IntervalTypes intervalTypes);
        Task<List<TickerQueries.Ticker>> GetTickersLimitedCandles(int candleLimit, IntervalTypes intervalType);
    }
}
